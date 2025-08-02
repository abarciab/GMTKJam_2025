﻿/*
Copyright (c) Omar Duarte
Unauthorized copying of this file, via any medium is strictly prohibited.
Writen by Omar Duarte.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System.Linq;
using UnityEngine;

namespace PluginMaster
{
    public static class MeshUtils
    {
        public static bool IsPrimitive(Mesh mesh)
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(mesh);
            return assetPath.Length < 6 ? false : assetPath.Substring(0, 6) != "Assets";
        }

        public static Collider AddCollider(Mesh mesh, GameObject target)
        {
            Collider collider = null;
            void AddMeshCollider()
            {
                var meshCollider = target.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
                collider = meshCollider;
            }
            if (IsPrimitive(mesh))
            {
                if (mesh.name == "Sphere") collider = target.AddComponent<SphereCollider>();
                else if (mesh.name == "Capsule") collider = target.AddComponent<CapsuleCollider>();
                else if (mesh.name == "Cube") collider = target.AddComponent<BoxCollider>();
                else if (mesh.name == "Plane") AddMeshCollider();
            }
            else AddMeshCollider();
            return collider;
        }

        public static GameObject[] FindFilters(LayerMask mask, GameObject[] exclude = null, bool excludeColliders = true)
        {
            var objects = new System.Collections.Generic.HashSet<GameObject>();
#if UNITY_2022_2_OR_NEWER
            var meshFilters = GameObject.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None);
            var skinnedMeshes = GameObject.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
#else
            var meshFilters = GameObject.FindObjectsOfType<MeshFilter>();
            var skinnedMeshes = GameObject.FindObjectsOfType<SkinnedMeshRenderer>();
#endif
            foreach ( var meshFilter in meshFilters) objects.Add(meshFilter.gameObject);
            foreach (var skinnedMesh in skinnedMeshes) objects.Add(skinnedMesh.gameObject);

            bool maskFilter(GameObject obj) => (mask.value & (1 << obj.layer)) != 0;
            var filterList = new System.Collections.Generic.List<GameObject>(objects);
            if (exclude != null)
            {
                foreach(var o in objects)
                {
                    if(!maskFilter(o)) continue;
                    if (exclude.Contains(o)) continue;
                    filterList.Add(o);
                }
                objects = new System.Collections.Generic.HashSet<GameObject>(filterList);
            }
            if (excludeColliders)
            {
#if UNITY_2022_2_OR_NEWER
                var colliders = GameObject.FindObjectsByType<Collider>(FindObjectsSortMode.None);
#else
                var colliders = GameObject.FindObjectsOfType<Collider>();
#endif
                var collidersSet = new System.Collections.Generic.HashSet<GameObject>();
                foreach( var c in colliders) collidersSet.Add(c.gameObject);
                filterList = new System.Collections.Generic.List<GameObject>();
                foreach (var o in objects)
                {
                    if (!maskFilter(o)) continue;
                    if (collidersSet.Contains(o)) continue;
                    filterList.Add(o);
                }
            }
            return filterList.ToArray();
        }

        public static bool Raycast(Ray ray, out RaycastHit hitInfo,
            out GameObject collider, System.Collections.Generic.IEnumerable<GameObject> filters, float maxDistance,
            bool sameOriginAsRay = true, Vector3 origin = new Vector3())
        {
            collider = null;
            hitInfo = new RaycastHit();
            hitInfo.distance = maxDistance;

            var minDistance = maxDistance;
            var resultHitNormal = Vector3.zero;
            var result = false;
            var hitPoint = Vector3.zero;
            var originPlane = new Plane(-ray.direction, origin);
            foreach (var filter in filters)
            {
                if (filter == null) continue;
                if (RayIntersectsGameObject(ray, filter, includeInactive: false, out hitPoint,
                    out float hitDistance, out Vector3 hitNormal))
                {
                    if (!sameOriginAsRay) hitDistance = originPlane.GetDistanceToPoint(hitPoint);
                    if (hitDistance > minDistance) continue;
                    result = true;
                    collider = filter;
                    minDistance = hitDistance;
                    resultHitNormal = hitNormal;
                }
            }
            if (result)
            {
                hitInfo.point = hitPoint;
                hitInfo.distance = minDistance;
                hitInfo.normal = resultHitNormal;
            }
            return result;
        }

        public static bool Raycast(Vector3 origin, Vector3 direction,
            out RaycastHit hitInfo, out GameObject collider, GameObject[] filters, float maxDistance)
        {
            var ray = new Ray(origin, direction);
            return Raycast(ray, out hitInfo, out collider, filters, maxDistance);
        }

        public static bool RaycastAll(Ray ray, System.Collections.Generic.List<RaycastHit> outHitInfoList,
            System.Collections.Generic.List<GameObject> outColliderList,
            System.Collections.Generic.IEnumerable<GameObject> filters, float maxDistance,
            bool sameOriginAsRay = true, Vector3 origin = new Vector3())
        {
            foreach (var filter in filters)
            {
                if (Raycast(ray, out RaycastHit hit, out GameObject collider,new GameObject[]{ filter },
                    maxDistance, sameOriginAsRay, origin))
                {
                    if (hit.distance > maxDistance) continue;
                    outHitInfoList.Add(hit);
                    outColliderList.Add(filter);
                }
            }
            return outHitInfoList.Count > 0;
        }


        const string _meshRayIntersectComputeShaderPath = "Shaders/MeshRayIntersect";
        static ComputeShader _meshRayIntersectComputeShader;
        public static bool RayIntersectsMesh(Ray ray, Mesh mesh, Transform meshTransform, out Vector3 hitPoint,
            out float distance, out Vector3 localNormal)
        {
            distance = 0f;
            localNormal = Vector3.zero;
            hitPoint = ray.origin;

            if (_meshRayIntersectComputeShader == null)
                _meshRayIntersectComputeShader = Resources.Load<ComputeShader>(_meshRayIntersectComputeShaderPath);
            int kFindMinT = _meshRayIntersectComputeShader.FindKernel("CSFindMinT");
            int kFindMinIndex = _meshRayIntersectComputeShader.FindKernel("CSFindMinIndex");
            int kComputeNorm = _meshRayIntersectComputeShader.FindKernel("CSComputeNormal");

            Vector3 localOrigin = meshTransform.InverseTransformPoint(ray.origin);
            Vector3 localDirection = meshTransform.InverseTransformDirection(ray.direction).normalized;

            Vector3[] verts = mesh.vertices;
            int[] tris = mesh.triangles;
            int triCount = tris.Length / 3;
            int groups = Mathf.CeilToInt(triCount / 64f);

            var vertBuf = new ComputeBuffer(verts.Length, sizeof(float) * 3);
            var triBuf = new ComputeBuffer(tris.Length, sizeof(int));
            var distBuf = new ComputeBuffer(1, sizeof(uint));
            var idxBuf = new ComputeBuffer(1, sizeof(uint));
            var normBuf = new ComputeBuffer(1, sizeof(float) * 3);

            vertBuf.SetData(verts);
            triBuf.SetData(tris);
            distBuf.SetData(new uint[] { 0x7F7FFFFFu });
            idxBuf.SetData(new uint[] { System.UInt32.MaxValue });
            normBuf.SetData(new Vector3[] { Vector3.zero });

            System.Action<int> BindCommon = kernel =>
            {
                _meshRayIntersectComputeShader.SetBuffer(kernel, "vertices", vertBuf);
                _meshRayIntersectComputeShader.SetBuffer(kernel, "triangles", triBuf);
                _meshRayIntersectComputeShader.SetVector("rayOrigin", localOrigin);
                _meshRayIntersectComputeShader.SetVector("rayDirection", localDirection);
            };

            BindCommon(kFindMinT);
            _meshRayIntersectComputeShader.SetBuffer(kFindMinT, "minDistanceBits", distBuf);
            _meshRayIntersectComputeShader.Dispatch(kFindMinT, groups, 1, 1);

            uint[] distBits = new uint[1];
            distBuf.GetData(distBits);
            float localT = System.BitConverter.ToSingle(System.BitConverter.GetBytes((int)distBits[0]), 0);
            if (localT == float.MaxValue)
            {
                vertBuf.Release(); triBuf.Release();
                distBuf.Release(); idxBuf.Release(); normBuf.Release();
                return false;
            }

            BindCommon(kFindMinIndex);
            _meshRayIntersectComputeShader.SetBuffer(kFindMinIndex, "minDistanceBits", distBuf);
            _meshRayIntersectComputeShader.SetBuffer(kFindMinIndex, "minTriangleIndex", idxBuf);
            _meshRayIntersectComputeShader.Dispatch(kFindMinIndex, groups, 1, 1);

            _meshRayIntersectComputeShader.SetBuffer(kComputeNorm, "vertices", vertBuf);
            _meshRayIntersectComputeShader.SetBuffer(kComputeNorm, "triangles", triBuf);
            _meshRayIntersectComputeShader.SetBuffer(kComputeNorm, "minTriangleIndex", idxBuf);
            _meshRayIntersectComputeShader.SetBuffer(kComputeNorm, "hitNormalBuffer", normBuf);
            _meshRayIntersectComputeShader.Dispatch(kComputeNorm, 1, 1, 1);

            Vector3[] normals = new Vector3[1];
            normBuf.GetData(normals);

            vertBuf.Release(); triBuf.Release();
            distBuf.Release(); idxBuf.Release(); normBuf.Release();

            Vector3 localHitPoint = localOrigin + localDirection * localT;
            hitPoint = meshTransform.TransformPoint(localHitPoint);
            distance = Vector3.Distance(ray.origin, hitPoint);
            localNormal = normals[0].normalized;
            return true;
        }

        public static (Mesh mesh, Transform transform)[] GetAllMeshses(GameObject obj, bool includeInactive)
        {
            var result = new System.Collections.Generic.HashSet<(Mesh, Transform)>();
            var meshFilters = obj.GetComponentsInChildren<MeshFilter>(includeInactive);
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh == null) continue;
                var renderer = mf.GetComponent<MeshRenderer>();
                if (!renderer.enabled) continue;
                result.Add((mf.sharedMesh, mf.transform));
            }
            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
            foreach (var smr in skinnedMeshRenderers)
            {
                if (smr.sharedMesh == null || !smr.enabled) continue;
                result.Add((smr.sharedMesh, smr.transform));
            }
            return result.ToArray();
        }
        public static bool RayIntersectsGameObject(Ray ray, GameObject gameObject, bool includeInactive, out Vector3 hitPoint,
            out float distance, out Vector3 hitNormal)
        {
            distance = float.MaxValue;
            hitNormal = Vector3.zero;
            hitPoint = ray.origin;
            var hitAny = false;
            var meshesAndTransforms = GetAllMeshses(gameObject, includeInactive);
            foreach (var mt in meshesAndTransforms)
            {
                if (mt.mesh == null) continue;
                if (RayIntersectsMesh(ray, mt.mesh, mt.transform, out Vector3 p, out float d, out Vector3 localN))
                {
                    hitAny = true;
                    hitPoint = p;
                    distance = Mathf.Min(distance, d);
                    hitNormal = mt.transform.TransformDirection(localN).normalized;
                }
            }
            if (!hitAny)
            {
                distance = 0f;
                return false;
            }
            return true;
        }

    }
}


using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Bear
{
    public class ResourceLoaderNodeData : ANodeData
    {
        public Dictionary<string, IResource> allResource = new Dictionary<string, IResource>();
        
    }

    public static class ResourceLoaderNodeDataSystem {
        public static void Load<T>(this ResourceLoaderNodeData resourcenodeData, string key, System.Action<T> DOnComplete)
        {
            var allResource = resourcenodeData.allResource;
            if (allResource.TryGetValue(key, out var value))
            {
                value.LoadResource<T>(DOnComplete);
            }
            else
            {
                var resource = new Resource(key);
                allResource[key] = resource;
                resource.LoadResource<T>(DOnComplete);
            }
        }

        
        public async static UniTask<T> LoadAsync<T>(this ResourceLoaderNodeData resourcenodeData, string key)
        {
            var allResource = resourcenodeData.allResource;
            if (allResource.TryGetValue(key, out var value))
            {
                var rs = await value.LoadResourceAsync<T>(key);
                return rs;
            }
            else
            {
                var resource = new Resource(key);
                allResource[key] = resource;

                var rs = await resource.LoadResourceAsync<T>(key);
                return rs;
            }
        }


        public static void UnloadResource(this ResourceLoaderNodeData resourcenodeData,string key)
        {
            var allResource = resourcenodeData.allResource;
            if (allResource.TryGetValue(key, out var value))
            {
                value.UnloadResource();
            }
        }
    }

    

    public interface IResource
    {
        void LoadResource<T>(System.Action<T> DOnComplete);

        UniTask<T> LoadResourceAsync<T>(string address);

        void UnloadResource();
    }

    

    public class Resource : IResource
    {
        public string ResourceName;
        object resource;
        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle handle;

        public Resource(string rname)
        {
            ResourceName = rname.Replace("\r", "");
        }
        private UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<T> LoadResource<T>()
        {
            // Debug.Log(ResourceName);
            var rs = Addressables.LoadAssetAsync<T>(ResourceName);
            rs.Completed += (rs) => {
                resource = rs.Result;
                handle = rs;
            };
            return rs;
        }

        public void LoadResource<T>(System.Action<T> DOnComplete)
        {
            if (resource != null && resource is T rs)
            {
                DOnComplete?.Invoke(rs);
            }
            else
            {
                LoadResource<T>().Completed += (rs) => {
                    DOnComplete?.Invoke(rs.Result);
                    resource = rs.Result;
                };
            }

        }

        public void UnloadResource()
        {
            resource = null;
            Addressables.Release(handle);
        }

        public async UniTask<T> LoadResourceAsync<T>(string address)
        {
            if (resource != null && resource is T rs)
            {
                return rs;
            }else{
                

                //var res = await Addressables.LoadAssetAsync<T>(address).ToUniTask(Progress.Create<float>(x => Debug.Log(x)));
                //return res;
                var res = await Addressables.LoadAssetAsync<T>(address);
                return res;
            }
        }

        
    }

}
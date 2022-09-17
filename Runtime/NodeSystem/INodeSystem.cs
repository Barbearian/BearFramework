using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear
{
    public static class INodeSystem
    {
        public static INode GlobalNode = new ANode();
        public static Dictionary<INode,List<INode>> Children = new Dictionary<INode, List<INode>>();
        public static Dictionary<INode,INode> Parent = new Dictionary<INode, INode>();

        public static void AddChildrenNode(this INode parent, INode kid){
            if(parent == null){
                return;
            }

            if(Parent.TryGetValue(kid,out var oldParent)){
                oldParent.RemoveChildrenNode(kid);
            }

            
            Parent[kid] = parent;
            Children.Enqueue<INode,INode>(parent,kid);
        }

        public static void RemoveChildrenNode(this INode parent, INode kid){
            Parent.Remove(kid);
            Children.Dequeue<INode,INode>(parent,kid);
        }

        public static bool TryGetKidNode<T>(this INode node, out T kid) where T:INode{
            kid = default;
            if(Children.TryGetValue(node,out var list)){
                foreach(var item in list){
                    if(item is T target){
                        kid = target;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool TryGetKidNode<T>(this INode node, int index,out T kid) where T:INode{
            kid = default;
            if(Children.TryGetValue(node,out var list)){
                if(index>=0 && index<list.Count){
                    if(list[index] is T rs){
                        kid = rs;
                        return true;
                    }
                    
                }
            }
            return false;
        }

        public static bool TryGetKidNode(this INode node, int index,out INode kid){
            kid = default;
            if(Children.TryGetValue(node,out var list)){
                if(index>=0 && index<list.Count){
                    kid = list[index];
                    return true;
                }
            }
            return false;
        }

        public static void SetParentNode(this INode kid, INode parent){
            parent.AddChildrenNode(kid);
        }

        public static void Dispose(this INode node){
            if(Parent.TryGetValue(node,out var parent)){
                parent.RemoveChildrenNode(node);
            }

            if(Children.TryGetValue(node, out var children)){
                Children.Remove(node);
                foreach(var kid in children){
                    kid.Dispose();
                }
            }
        }

        

        public static INodeData AddNodeData(this INode node, INodeData data)
        {
            var key = data.GetType();
            var NodeData = node.GetNodeDataCollection();
            var NodeDataRequests = node.GetNodeDataRequestCollection();

            data.SetNodeDataRoot(node);

            NodeData[key] = data;

            if (NodeDataRequests.TryGetValue(key, out var requests))
            {
                foreach (var request in requests)
                {
                    request.Invoke(data);
                }

                NodeDataRequests.Remove(key);
            }

            return data;
        }

        public static bool RequestNodeData<T>(this INode node,System.Action<T> DOnDataRequested) where T:INodeData{
            var key = typeof(T);
            var NodeData = node.GetNodeDataCollection();
            var NodeDataRequests = node.GetNodeDataRequestCollection();
            if (NodeData.TryGetValue(key, out var value))
            {
                if (value is T tNodeData) {
                    DOnDataRequested?.Invoke(tNodeData);
                }
                
            }
            else {
                NodeDataRequests.Enqueue(
                    key,
                    
                    (nodeData)=> {
                    if (nodeData is T tNodeData)
                    {
                        DOnDataRequested?.Invoke(tNodeData);
                    }

                });
            }
            return true;
        }

        public static bool TryGetNodeData<T>(this INode node,out T data) where T:INodeData{
            var NodeData = node.GetNodeDataCollection();
            if(NodeData.TryGetValue(typeof(T),out var ndata)){
                data = (T)ndata;
                return true;
            }else{
                data = default;
                return false;
            }
        }

        public static T GetOrCreateNodeData<T>(this INode node, INodeData defaultNode) where T:INodeData{
            var NodeData = node.GetNodeDataCollection();
            if(NodeData.TryGetValue(typeof(T),out var ndata)){
                return (T)ndata;
            }else{
                return (T)node.AddNodeData(defaultNode);
            }
        }

        public static bool TryGetGlobalNodeData<T>(this IGlobalNodeDataAccessor accessor, out T data) where T: INodeData{

            return GlobalNode.TryGetNodeData<T>(out data);
        }

        public static INodeData AddGlobalNodeData<T>(this IGlobalNodeDataAccessor accessor, INodeData data) where T : INodeData
        {
            return GlobalNode.AddNodeData(data);
        }

        public static INodeData AddGlobalNodeData(this IGlobalNodeDataAccessor accessor, INodeData data)
        {
            return GlobalNode.AddNodeData(data);
        }



        public static bool RequestGlobalNodeData<T>(this IGlobalNodeDataAccessor requestor,System.Action<T> request) where T : INodeData {
            return GlobalNode.RequestNodeData(request);
        }



        public static void Enqueue<K,V>(this Dictionary<K, List<V>> requests,K key,V requestor)
        {
            if (requests.TryGetValue(key, out var requestors))
            {
                requestors.Add(requestor);
            }
            else {
                requests[key] = new List<V>() { requestor};
            }
        }

        public static void Dequeue<K,V>(this Dictionary<K, List<V>> requests,K key,V value)
        {
            if (requests.TryGetValue(key, out var queue))
            {
                queue.Remove(value);
            }
            
        }
    }

    public interface IGlobalNodeDataAccessor { 
    }


}
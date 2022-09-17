using System.Collections;
using System.Collections.Generic;
using System;

namespace Bear{
    public static class INodeDataSystem 
    {
        public static Dictionary<INode,NodeInfo> nodeInfo = new Dictionary<INode, NodeInfo>();
        public static Dictionary<INodeData,NodeDataInfo> nodeDataInfo = new Dictionary<INodeData,NodeDataInfo>();
        public static Dictionary<Type,INodeData> GetNodeDataCollection(this INode node){
            if(nodeInfo.TryGetValue(node,out NodeInfo info)){
                return info.nodeData;
            }else{
                NodeInfo ninfo = new NodeInfo(){
                    nodeData = new Dictionary<Type, INodeData>(),
                    requests = new Dictionary<Type, List<Action<INodeData>>>()         
                };
                nodeInfo[node] = ninfo;
                return ninfo.nodeData;
            }
        }

        public static Dictionary<Type,List<Action<INodeData>>> GetNodeDataRequestCollection(this INode node){
            if(nodeInfo.TryGetValue(node,out NodeInfo info)){
                return info.requests;
            }else{
                NodeInfo ninfo = new NodeInfo(){
                    nodeData = new Dictionary<Type, INodeData>(),
                    requests = new Dictionary<Type, List<Action<INodeData>>>()         
                };
                nodeInfo[node] = ninfo;
                return ninfo.requests;
            }
        }

        public static void DisposeNodeInfo(this INode node){
            nodeInfo.Remove(node);

            // if(node is IDisposable disposable){
            //     disposable.Dispose();
            // }
        }

        public static INode GetNodeDataRoot(this INodeData nodeData){
            if(nodeDataInfo.TryGetValue(nodeData,out var info)){
                return info.Root;
            }else{
                return null;
            }
        }

        public static void SetNodeDataRoot(this INodeData nodeData, INode root){
             if(nodeDataInfo.TryGetValue(nodeData,out var info)){
                info.Root = root;
            }else{
                var nnodeDataInfo = NodeDataInfo.Create();
                nnodeDataInfo.Root = root;
                nodeDataInfo[nodeData] = nnodeDataInfo;
            }
        }

        public static void DisposeNodeDataInfo(this INodeData nodedata){
            nodeDataInfo.Remove(nodedata);
        }
    }

    public struct NodeInfo{
        public Dictionary<Type,INodeData> nodeData;
        public Dictionary<Type,List<Action<INodeData>>> requests;
    }

    public struct NodeDataInfo{
        public INode Root;

        public static NodeDataInfo Create(){
            return new NodeDataInfo(){
                
            };
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear{
    public interface IOnAttachedToNode{
        public void Attached(INode node);
    }

    public interface IOnDetachedFromNode{
        public void Detached(INode node);
    }

    public static class NodeDataLifeCycleSystem{
        public static void OnAttched(this INodeData nodedata, INode root){
            if(nodedata is IOnAttachedToNode idata && root != null){
                idata.Attached(root);
            }
        }

        public static void OnDetached(this INodeData nodedata, INode root){
            if(nodedata is IOnDetachedFromNode idata && root != null){
                idata.Detached(root);
            }
        }
    }
}
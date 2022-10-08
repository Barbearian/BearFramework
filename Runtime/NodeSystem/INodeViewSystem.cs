using UnityEngine;
namespace Bear
{

    public static class INodeViewSystem 
    {
        

        //You may add one kind of node view to an gameobject
        public static T AddNodeView<T>(this GameObject gameObject) where T:NodeView
        {
            if(gameObject.TryGetComponent<T>(out var nodeView)){
                return nodeView;
            }else{
                return gameObject.AddComponent<T>();
            }

        }

        public static void AddChildrenAtZero(this Transform parent, Transform kid){
            kid.parent = parent;

            //kid.SetParent(parent);
            kid.localPosition = Vector3.zero;
            kid.localRotation = Quaternion.identity;
        }

        public static void AddNodeViewChild(this NodeView parent, NodeView kid){
            parent.AddChildrenNode(kid);
            parent.transform.AddChildrenAtZero(kid.transform);
        }

        public static void AddNodeOrNodeViewChild(this NodeView parent, object kid){
            if(kid is NodeView view){
                parent.AddNodeViewChild(view);
            }else if(kid is INode node){
                parent.AddChildrenNode(node);
            }
        }
        
    }

}
    
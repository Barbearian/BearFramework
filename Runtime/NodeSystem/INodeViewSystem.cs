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

        
    }

}
    
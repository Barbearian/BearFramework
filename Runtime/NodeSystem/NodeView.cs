using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear{
    public class NodeView : MonoBehaviour, INode
    {
        public ANode anode = new ANode();
        

        public void Dispose(){
           anode.Dispose();
        }

        private void OnDestroy() {
            Dispose();    
        }
    }
}
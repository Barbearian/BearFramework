using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bear
{
    public interface INode
    {
               
    }

    public struct ANode : INode
    {

    }

    public class ANodeClass: INode{
       
    }



    public interface INodeData {

    }

    public class ANodeData : INodeData
    {
        private INode _root;
        public INode Root => _root;
    }
}
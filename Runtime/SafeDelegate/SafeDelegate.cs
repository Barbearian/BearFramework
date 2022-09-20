using UnityEngine;
using System;
namespace Bear{
    public class SafeDelegate 
    {
        public System.Action invoker;
        
        public static Action operator +(SafeDelegate self,Action action){
            return self.Link(action);
        }
    }

    public class SafeDelegate<T>
    {
        public System.Action<T> invoker;
        public static Action operator +(SafeDelegate<T> self,Action<T> action){
            return self.Link(action);
        }
    }

    public struct SafeDelegateBridge<T>{
        public SafeDelegate<T> invoker;
        public Action<T> invoked;

        public void Cancel(){
            if(invoker != null && invoker.invoker != null){
                invoker.invoker -= invoked;
            }
            
        }
    }

    public struct SafeDelegateBridge{
        public SafeDelegate invoker;
        public Action invoked;

        public void Cancel(){
            invoker.invoker -= invoked;
        }
    }

    public static class SafeDelegateSystem{


        public static Action Link<T>(this SafeDelegate<T> invoker,Action<T> invoked){
            SafeDelegateBridge<T> bridge = new SafeDelegateBridge<T>();
            bridge.invoker = invoker;
            Action<T> invokedAction = (x)=>{
                try{
                    invoked(x);
                }catch(Exception e){
                    Debug.LogWarning(e);
                    bridge.Cancel();
                };
           };
           invoker.invoker += invokedAction;
           bridge.invoked = invokedAction;
           
           return bridge.Cancel;
            
        }

        public static Action Link(this SafeDelegate invoker,Action invoked){
            SafeDelegateBridge bridge = new SafeDelegateBridge();
            bridge.invoker = invoker;
            Action invokedAction = ()=>{
                try{
                    invoked();
                }catch(Exception e){
                    Debug.LogWarning(e);
                    bridge.Cancel();
                };
           };
           invoker.invoker += invokedAction;
           bridge.invoked = invokedAction;
           
           return bridge.Cancel;
            
        }

        


    }

    
}
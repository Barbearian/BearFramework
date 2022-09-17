using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using UnityEngine;

namespace Bear
{
    public sealed class BearAEventSystem
    {
        private static BearAEventSystem _instance;
        public static BearAEventSystem Instance {
            get {
                if (_instance == null) { 
                    _instance = new BearAEventSystem(); 
                }
                return _instance;
            }
        }
        private readonly Dictionary<Type, List<IHandler>> allEvents = new Dictionary<Type, List<IHandler>>();

        public void Add(IEnumerable<Type> types) {
            allEvents.Clear();

            foreach (Type type in types) {
                Add(type);
            }
        }


        public void Add(IEnumerable<Assembly> assemblies)
        {
            // this.assemblies[$"{assembly.GetName().Name}.dll"] = assembly;

            // Dictionary<string, Type> dictionary = new Dictionary<string, Type>();

            foreach (var ass in assemblies)
            {
                this.Add(ass);
            }



        }
        public void Add(Assembly assembly)
        {
            // this.assemblies[$"{assembly.GetName().Name}.dll"] = assembly;

            // Dictionary<string, Type> dictionary = new Dictionary<string, Type>();

            foreach (Type type in assembly.GetTypes())
            {
                this.Add(type);
            }
            

           
        }

        public void Add(Type type) {
            if (typeof(IBearEvent).IsAssignableFrom(type) && !type.IsAbstract) {
                var instance = Activator.CreateInstance(type);
                if (instance is IBearEvent handler) { 
                    

                    var listenedType = handler.GetEventType();
                    AddNewType(listenedType, handler);
                }
            }
        }



        private void AddNewType(Type ListenedType, IHandler selfType) {
            if (allEvents.TryGetValue(ListenedType, out var list))
            {
                if (!list.Contains(selfType))
                {
                    list.Add(selfType);
                }
            }
            else {
                allEvents[ListenedType] = new List<IHandler>() {selfType};
            }
            
            
            
        }

        public void Publish<T>(T a)
        {
            if (allEvents.TryGetValue(a.GetType(),out var handlers)) {
                foreach (var handler in handlers)
                {
                    handler.Handle(a);
                }
            }
        }

        
    }

}
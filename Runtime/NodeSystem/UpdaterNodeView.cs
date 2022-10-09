using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Bear
{
    public class UpdaterNodeView : NodeView, IOnFixedUpdateUpdater, IOnUpdateUpdater
    {
        public Updater DOnFixedUpdate { get; private set; } = new Updater();
        public Updater DOnUpdate { get; private set; } = new Updater();
        public void Update()
        {
            DOnUpdate.Update();
        }

        public void FixedUpdate()
        {
            DOnFixedUpdate.Update();
        }
    }

    public interface IUpdater { 
        public void Update();
        public void Subscribe(Action action);
        public void Unsubscribe(Action action);
        public void Dispose();
    }
    public class Updater : IUpdater
    {
        private bool active;
        public Action update;
        public void Dispose()
        {
            active = false;
            update = null;
        }

        public void Subscribe(Action action)
        {
            active = true;
            update += action;
        }

        public void Unsubscribe(Action action)
        {
            update -= action;
            active = update != null;
        }

        public void Update()
        {
            if (active) {
                update.Invoke();
            }
            
        }
    }

    public interface IOnFixedUpdateUpdater { 
        public Updater DOnFixedUpdate { get; }
    }

    public interface IOnUpdateUpdater
    {
        public Updater DOnUpdate { get; }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Bear
{
    public class CodeLoader : IDisposable
    {
        public static CodeLoader Instance = new CodeLoader();

		// all mono
		private readonly Dictionary<string, Type> monoTypes = new Dictionary<string, Type>();

		//all hotfix
		private readonly Dictionary<string, Type> hotfixTypes = new Dictionary<string, Type>();
		private CodeLoader() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly ass in assemblies)
			{
				foreach (Type type in ass.GetTypes())
				{
					this.monoTypes[type.FullName] = type;
					this.monoTypes[type.AssemblyQualifiedName] = type;
				}
			}

			
		}

		public static void printInfo() {
			Debug.Log("LOL");
		}

		public Type GetMonoType(string fullName)
		{
			this.monoTypes.TryGetValue(fullName, out Type type);
			return type;
		}

		public Type GetHotfixType(string fullName)
		{
			this.hotfixTypes.TryGetValue(fullName, out Type type);
			return type;
		}

		public IEnumerable<Type> GetAllType() {
			return monoTypes.Values;
		}

		public void Dispose()
        {
        }
    }
}
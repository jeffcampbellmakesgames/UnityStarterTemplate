using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace JCMG.Utility.Extended
{
    /// <summary>
    	/// Helper methods for <see cref="ISymbolObject"/>.
    	/// </summary>
    	public static class ISymbolObjectExtensions
    	{
    		/// <summary>
    		/// Returns true if a <typeparamref name="TSymbolObject"/> can be found for <paramref name="symbol"/>, otherwise
    		/// false. If true, <paramref name="symbolObject"/> will be initialized.
    		/// </summary>
    		public static bool TryGet<TSymbolObject>(
    			this IList<TSymbolObject> symbolObjects,
    			string symbol,
    			out TSymbolObject symbolObject)
    			where TSymbolObject : class, ISymbolObject
    		{
    			symbolObject = null;
    
    			for (var i = 0; i < symbolObjects.Count; i++)
    			{
    				if (!Equals(symbolObjects[i].Symbol, symbol))
    				{
    					continue;
    				}
    
    				symbolObject = symbolObjects[i];
    				break;
    			}
    
    			return symbolObject != null;
    		}
    
    		/// <summary>
    		/// Returns a <typeparamref name="TSymbolObject"/> for matching <paramref name="symbol"/>.
    		/// </summary>
    		/// <exception cref="Exception">Throws an exception if a <typeparamref name="TSymbolObject"/> is not found
    		/// for matching <paramref name="symbol"/>.</exception>
    		public static TSymbolObject Get<TSymbolObject>(
    			this IList<TSymbolObject> keyedObjects,
    			string symbol)
    			where TSymbolObject : class, ISymbolObject
    		{
    			TSymbolObject symbolObject = null;
    
    			for (var i = 0; i < keyedObjects.Count; i++)
    			{
    				if (!Equals(keyedObjects[i].Symbol, symbol))
    				{
    					continue;
    				}
    
    				symbolObject = keyedObjects[i];
    				break;
    			}
    
    			Assert.IsNotNull(symbolObject);
    
    			if (symbolObject == null)
    			{
    				throw new Exception($"No {typeof(TSymbolObject).Name} can be found for symbol value {symbol}");
    			}
    
    			return symbolObject;
    		}
    	}
}

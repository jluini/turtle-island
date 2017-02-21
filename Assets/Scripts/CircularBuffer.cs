
using System;
using UnityEngine;

namespace JuloUtil {
	public class CircularBuffer<T> {
		
		private T[] arr;
		private int maxSize;
		
		private bool empty = true;
		private int startIndex = 0;
		private int endIndex = 0;
		
		public CircularBuffer(int maxSize) {
			this.maxSize = maxSize;
			if(maxSize < 2)
				throw new ApplicationException("Invalid maxSize");
			
			this.arr = new T[maxSize];
		}
		
		public void clear() {
			this.empty = true;
			this.startIndex = 0;
			this.endIndex = 0;
		}
		
		public void save(T elem) {
			if(empty) {
				empty = false;
				if(endIndex != startIndex)
					Debug.LogWarning("Should be equal?");
				endIndex = startIndex;
			} else {
				endIndex = (endIndex + 1) % maxSize;
				if(startIndex == endIndex) {
					startIndex = (startIndex + 1) % maxSize;
				}
			}
			
			arr[endIndex] = elem;
			
			//Debug.Log(startIndex + ":" + endIndex + "--=>--" + oldSize + " -> " + getSize());
		}
		
		public int getSize() {
			if(empty) {
				return 0; 
			} else if(startIndex <= endIndex) {
				return endIndex - startIndex + 1;
			} else {
				return endIndex - startIndex + maxSize + 1;
			}
		}
		
		public T[] getSequence() {
			int size = getSize();
			T[] ret = new T[size];
			
			for(int i = 0; i < size; i++) {
				ret[i] = arr[(startIndex + i) % maxSize];
			}
			
			return ret;
		}
	}
}
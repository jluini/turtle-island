
using System;
using UnityEngine;

namespace JuloUtil {
	public class CircularBuffer<T> {
		private T[] arr;
		private int maxSize;
		
		private int _length = 0;
		public int length {
			get {
				return _length;
			}
		}
		
		private int startIndex = 0;
		private int endIndex = 0;
		
		public CircularBuffer(int maxSize) {
			this.maxSize = maxSize;
			if(maxSize < 2)
				throw new ApplicationException("Invalid maxSize");
			
			this.arr = new T[maxSize];
		}
		
		public void clear() {
			this._length = 0;
			this.startIndex = 0;
			this.endIndex = 0;
		}
		
		public void save(T elem) {
			if(_length == 0) {
				_length = 1;
				if(endIndex != startIndex)
					Debug.LogWarning("Should be equal");
				endIndex = startIndex;
			} else if(_length > 0) {
				endIndex = (endIndex + 1) % maxSize;
				if(startIndex == endIndex) {
					startIndex = (startIndex + 1) % maxSize;
				} else {
					_length++;
				}
			} else {
				throw new ApplicationException("negative length");
			}
			
			arr[endIndex] = elem;
		}
		
		public T elemToOverride() {
			if(_length == 0) {
				return arr[endIndex];
			} else {
				return arr[(endIndex + 1) % maxSize];
			}
		}
		
		public T get(int index) {
			return arr[(startIndex + index) % maxSize];
		}
	}
}
﻿using System;
using System.Runtime.Serialization;
using Sudoku.Runtime;

namespace Sudoku.Recognitions
{
	/// <summary>
	/// Represents an error that the OCR tools cannot work normally.
	/// </summary>
	[Serializable]
	public class RecognitionException : SudokuRuntimeException
	{
		/// <inheritdoc/>
		public RecognitionException() : base() { }

		/// <inheritdoc/>
		public RecognitionException(string message) : base(message) { }

		/// <inheritdoc/>
		public RecognitionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <inheritdoc/>
		protected RecognitionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
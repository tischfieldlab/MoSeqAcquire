/*
 * Copyright � 2006 Stefan Trosch�tz (stefan@troschuetz.de)
 * 
 * This file is part of Troschuetz.Random Class Library.
 * 
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 * ExponentialDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 16.08.2006: Adjusted NextDouble so that Math.Log(0.0) is avoided
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

using System;
using MoSeqAcquire.Models.Utility.Random;

namespace MoSeqAcquire.Models.Utility.Random
{
	/// <summary>
	/// Provides generation of exponential distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="ExponentialDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Exponential_distribution">Wikipedia - Exponential distribution</a>.
    /// </remarks>
	public class ExponentialDistribution : Distribution
	{
		#region instance fields
		/// <summary>
		/// Gets or sets the parameter lambda which is used for generation of exponential distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidLambda"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Lambda
		{
			get
			{
				return this.lambda;
			}
			set
			{
                if (this.IsValidLambda(value))
                {
                    this.lambda = value;
                    this.UpdateHelpers();
                }
        	}
		}
		
		/// <summary>
		/// Stores the parameter lambda which is used for generation of exponential distributed random numbers.
		/// </summary>
		private double lambda;

        /// <summary>
        /// Stores an intermediate result for generation of exponential distributed random numbers.
        /// </summary>
        /// <remarks>
        /// Speeds up random number generation cause this value only depends on distribution parameters 
        ///   and therefor doesn't need to be recalculated in successive executions of <see cref="NextDouble"/>.
        /// </remarks>
        private double helper1;
        #endregion
		
		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public ExponentialDistribution()
            : this(new StandardGenerator())
		{
		}
		
		/// <summary>
        /// Initializes a new instance of the <see cref="ExponentialDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public ExponentialDistribution(Generator generator)
            : base(generator)
        {
            this.lambda = 1.0;
            this.UpdateHelpers();
        }
        #endregion
		
		#region instance methods
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Lambda"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidLambda(double value)
		{
			return value > 0.0;
		}
		
        /// <summary>
        /// Updates the helper variables that store intermediate results for generation of exponential distributed random 
        ///   numbers.
        /// </summary>
        private void UpdateHelpers()
        {
            this.helper1 = -1.0 / this.lambda;
        }
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of exponential distributed random numbers.
		/// </summary>
		public override double Minimum
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of exponential distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of exponential distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
				return 1.0 / this.lambda;
			}
		}
		
		/// <summary>
		/// Gets the median of exponential distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return Math.Log(2.0) / this.lambda;
			}
		}
		
		/// <summary>
		/// Gets the variance of exponential distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
				return Math.Pow(this.lambda, -2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of exponential distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
			get
			{
				return new double[] {0.0};
			}
		}

		/// <summary>
		/// Returns a exponential distributed floating point random number.
		/// </summary>
		/// <returns>A exponential distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            // Subtract random number from 1.0 to avoid Math.Log(0.0)
            return this.helper1 * Math.Log(1.0 - this.Generator.NextDouble());
		}
		#endregion
	}
}
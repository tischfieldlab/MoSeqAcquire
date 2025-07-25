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
 * LaplaceDistribution.cs, 21.09.2006
 * 
 * 17.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

using System;
using MoSeqAcquire.Models.Utility.Random;

namespace MoSeqAcquire.Models.Utility.Random
{
	/// <summary>
	/// Provides generation of laplace distributed random numbers.
	/// </summary>
	/// <remarks>
    /// The implementation of the <see cref="LaplaceDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Laplace_distribution">Wikipedia - Laplace distribution</a>.
    /// </remarks>
	public class LaplaceDistribution : Distribution
	{
		#region instance fields
		/// <summary>
		/// Gets or sets the parameter alpha which is used for generation of laplace distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidAlpha"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
                if (this.IsValidAlpha(value))
                {
                    this.alpha = value;
                }
        	}
		}

		/// <summary>
		/// Stores the parameter alpha which is used for generation of laplace distributed random numbers.
		/// </summary>
		private double alpha;
		
		/// <summary>
		/// Gets or sets the parameter mu which is used for generation of laplace distributed random numbers.
		/// </summary>
		/// <remarks>Call <see cref="IsValidMu"/> to determine whether a value is valid and therefor assignable.</remarks>
		public double Mu
		{
			get
			{
				return this.mu;
			}
			set
			{
                if (this.IsValidMu(value))
                {
                    this.mu = value;
                }
        	}
		}

		/// <summary>
		/// Stores the parameter mu which is used for generation of laplace distributed random numbers.
		/// </summary>
		private double mu;
        #endregion

		#region construction, destruction
		/// <summary>
        /// Initializes a new instance of the <see cref="LaplaceDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public LaplaceDistribution()
            : this(new StandardGenerator())
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="LaplaceDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public LaplaceDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 1.0;
            this.mu = 0.0;
        }
		#endregion
	
		#region instance methods
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0.0; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidAlpha(double value)
		{
			return value > 0;
		}
		
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Mu"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
        /// <returns><see langword="true"/>.</returns>
		public bool IsValidMu(double value)
		{
			return true;
		}
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of laplace distributed random numbers.
		/// </summary>
		public override double Minimum
		{
			get
			{
				return double.MinValue;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of laplace distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of laplace distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
				return this.mu;
			}
		}
		
		/// <summary>
		/// Gets the median of laplace distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
                return this.mu;
			}
		}
		
		/// <summary>
		/// Gets the variance of laplace distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return 2.0 * Math.Pow(this.alpha, 2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of laplace distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
            get
            {
                return new double[] { this.mu };
            }
		}
		
		/// <summary>
		/// Returns a laplace distributed floating point random number.
		/// </summary>
        /// <returns>A laplace distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
            double rand = 0.5 - this.Generator.NextDouble();
            return this.mu - this.alpha * Math.Sign(rand) * Math.Log(2.0 * Math.Abs(rand));
		}
		#endregion
	}
}
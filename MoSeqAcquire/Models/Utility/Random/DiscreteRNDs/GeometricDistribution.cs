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
 * GeometricDistribution.cs, 21.09.2006
 * 
 * 09.08.2006: Initial version
 * 21.09.2006: Adapted to change in base class (field "generator" declared private (formerly protected) 
 *               and made accessible through new protected property "Generator")
 * 
 */

#region original copyrights
//   -*- C++ -*-
/*****************************************************************************
 *
 *   |_|_|_  |_|_    |_    |_|_|_  |_		     C O M M U N I C A T I O N
 * |_        |_  |_  |_  |_        |_		               N E T W O R K S
 * |_        |_  |_  |_  |_        |_		                     C L A S S
 *   |_|_|_  |_    |_|_    |_|_|_  |_|_|_|_	                 L I B R A R Y
 *
 * $Id: Geometric.c,v 1.2 2002/01/14 11:37:33 spee Exp $
 *
 * CNClass: CNGeometric --- CNGeometric distributed random numbers
 *
 *****************************************************************************
 * Copyright (C) 1992-1996   Communication Networks
 *                           Aachen University of Technology
 *                           D-52056 Aachen
 *                           Germany
 *                           Email: cncl-adm@comnets.rwth-aachen.de
 *****************************************************************************
 * This file is part of the CN class library. All files marked with
 * this header are free software; you can redistribute it and/or modify
 * it under the terms of the GNU Library General Public License as
 * published by the Free Software Foundation; either version 2 of the
 * License, or (at your option) any later version.  This library is
 * distributed in the hope that it will be useful, but WITHOUT ANY
 * WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Library General Public
 * License for more details.  You should have received a copy of the GNU
 * Library General Public License along with this library; if not, write
 * to the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139,
 * USA.
 *****************************************************************************
 * original Copyright:
 * -------------------
 * Copyright (C) 1988 Free Software Foundation
 *    written by Dirk Grunwald (grunwald@cs.uiuc.edu)
 * 
 * This file is part of the GNU C++ Library.  This library is free
 * software; you can redistribute it and/or modify it under the terms of
 * the GNU Library General Public License as published by the Free
 * Software Foundation; either version 2 of the License, or (at your
 * option) any later version.  This library is distributed in the hope
 * that it will be useful, but WITHOUT ANY WARRANTY; without even the
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 * PURPOSE.  See the GNU Library General Public License for more details.
 * You should have received a copy of the GNU Library General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *****************************************************************************/
#endregion

using System;
using MoSeqAcquire.Models.Utility.Random;

namespace MoSeqAcquire.Models.Utility.Random
{
	/// <summary>
	/// Provides generation of geometric distributed random numbers.
	/// </summary>
	/// <remarks>
	/// The geometric distribution generates only discrete numbers.<br />
    /// The implementation of the <see cref="GeometricDistribution"/> type bases upon information presented on
    ///   <a href="http://en.wikipedia.org/wiki/Geometric_distribution">Wikipedia - Geometric distribution</a>
    ///   and the implementation in the <a href="http://www.lkn.ei.tum.de/lehre/scn/cncl/doc/html/cncl_toc.html">
    ///   Communication Networks Class Library</a>.
    /// </remarks>
	public class GeometricDistribution : Distribution
	{
		#region instance fields
		/// <summary>
        /// Gets or sets the parameter alpha which is used for generation of geometric distributed random numbers.
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
        /// Stores the parameter alpha which is used for generation of geometric distributed random numbers.
		/// </summary>
        private double alpha;
        #endregion

		#region construction
		/// <summary>
        /// Initializes a new instance of the <see cref="GeometricDistribution"/> class, using a 
        ///   <see cref="StandardGenerator"/> as underlying random number generator.
		/// </summary>
        public GeometricDistribution()
            : this(new StandardGenerator())
		{
		}
		
		/// <summary>
        /// Initializes a new instance of the <see cref="GeometricDistribution"/> class, using the specified 
        ///   <see cref="Generator"/> as underlying random number generator.
        /// </summary>
        /// <param name="generator">A <see cref="Generator"/> object.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="generator"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public GeometricDistribution(Generator generator)
            : base(generator)
        {
            this.alpha = 0.5;
        }
		#endregion
	
		#region instance methods
		/// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="Alpha"/>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns>
		/// <see langword="true"/> if value is greater than 0.0, and less than or equal to 1.0; otherwise, <see langword="false"/>.
		/// </returns>
        public bool IsValidAlpha(double value)
		{
			return (value > 0.0 && value <= 1.0);
		}

        /// <summary>
        /// Returns a geometric distributed random number.
        /// </summary>
        /// <returns>A geometric distributed 32-bit signed integer.</returns>
        public int Next()
        {
            int samples = 1;
            for (; this.Generator.NextDouble() >= this.alpha; samples++)
            {
            }

            return samples;
        }
        #endregion

		#region overridden Distribution members
        /// <summary>
		/// Gets the minimum possible value of geometric distributed random numbers.
		/// </summary>
        public override double Minimum
		{
			get
			{
				return 1.0;
			}
		}

		/// <summary>
		/// Gets the maximum possible value of geometric distributed random numbers.
		/// </summary>
        public override double Maximum
		{
			get
			{
				return double.MaxValue;
			}
		}

		/// <summary>
		/// Gets the mean value of geometric distributed random numbers.
		/// </summary>
        public override double Mean
		{
			get
			{
                return 1.0 / this.alpha;
			}
		}
		
		/// <summary>
		/// Gets the median of geometric distributed random numbers.
		/// </summary>
        public override double Median
		{
			get
			{
				return double.NaN;
			}
		}
		
		/// <summary>
		/// Gets the variance of geometric distributed random numbers.
		/// </summary>
        public override double Variance
		{
			get
			{
                return (1.0 - this.alpha) / Math.Pow(this.alpha, 2.0);
			}
		}
		
		/// <summary>
		/// Gets the mode of geometric distributed random numbers.
		/// </summary>
        public override double[] Mode
		{
			get
			{
				return new double[] {1.0};
			}
		}
		
		/// <summary>
		/// Returns a geometric distributed floating point random number.
		/// </summary>
		/// <returns>A geometric distributed double-precision floating point number.</returns>
        public override double NextDouble()
		{
			double samples = 1.0;
            for (; this.Generator.NextDouble() >= this.alpha; samples++)
            {
            }

			return samples;
		}
		#endregion
	}
}
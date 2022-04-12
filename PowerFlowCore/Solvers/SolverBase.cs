using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

using Complex = System.Numerics.Complex;

namespace PowerFlowCore.Data
{
   
    #region VARIANT 2 (STATIC METHODS)

    //Static class for solvers
    public static class Solvers
    {

        //static GS
        public static Grid SolverGS(this Grid grid,
                                    CalculationOptions options)
        {

            throw new NotImplementedException();
        }

        //static NR
        public static Grid SolverNR(this Grid grid,
                                    CalculationOptions options)
        {

            throw new NotImplementedException();
        }
    }


    #endregion


}

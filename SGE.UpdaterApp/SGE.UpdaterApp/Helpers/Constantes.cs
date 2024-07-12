﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace SGE.UpdaterApp.Helpers
{
    public class Constantes
    {
        public static int Connection = 0;
        public static int ConnGrenPeru = 1;
        public static int ConnGalyCompany = 2;
        public static int ConnMotoTorque = 3;
        public static int ConnNovaGlass = 4;
        public static int ConnNovaFlat = 5;
        public static int ConnNovaMotos = 6;
        public static int ConnCalzadosJaguar = 7;
        public static int ConnPradosVerdes = 8;
        public static int ConnTelasLima = 9;
        public static int ConnUlike = 10;
        public static int ConnMultiEmpresa = 11;
        public static int ConnJabsa = 12;

        public static int TotalConecciones = 12;
        //

        public static int msgAlert = 1;
        public static int msgError = 2;

        //

        public static string rutaInfotxt = @"\info.txt";
        public static string rutaCarpetaInfo = @"C:\InfoUpdate";

        //

        public static List<int> conneciones = new();

        public static int tabLogin = 0;
        public static int tabSeleccionar = 1;
        public static int tabInstalar = 2;
        public static int tabActualizar = 3;
    }
}

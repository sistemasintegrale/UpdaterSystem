using Microsoft.Data.SqlClient;
using SGE.UpdaterApp.Entities;
using SGE.UpdaterApp.Helpers;
using System.Data;

namespace SGE.UpdaterApp.DataAcces
{
    public class GeneralData
    {


        public List<Usuario> listarUsuarios()
        {
            List<Usuario> list = new();

            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SGES_USUARIO_LISTAR", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        cmd.Parameters.AddWithValue("@usua_secured_key", "ACCESSKEY");
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            list.Add(new Usuario
                            {
                                usua_icod_usuario = Convert.ToInt32(reader["usua_icod_usuario"]),
                                usua_codigo_usuario = reader["usua_codigo_usuario"].ToString()!.Trim(),
                                usua_nombre_usuario = reader["usua_nombre_usuario"].ToString()!.Trim(),
                                usua_password_usuario = reader["usua_password_usuario"].ToString()!,
                                usua_iactivo = Convert.ToBoolean(reader["usua_iactivo"]),
                                strEstado = Convert.ToBoolean(reader["usua_iactivo"]) ? "Activo" : "Inactivo",
                            });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return list;
        }

        internal void Equipo_Insertar(ControlEquipos objEquipo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ACT_EQUIPO_INSERTAR", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        cmd.Parameters.AddWithValue("@ceq_vnombre_equipo", objEquipo.ceq_vnombre_equipo);
                        cmd.Parameters.AddWithValue("@cvr_icod_version", objEquipo.cvr_icod_version);
                        cmd.Parameters.AddWithValue("@cep_vid_cpu", objEquipo.cep_vid_cpu);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void Equipo_Modificar(ControlEquipos objEquipo)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ACT_EQUIPO_MODIFICAR", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        cmd.Parameters.AddWithValue("@ceq_icod_equipo", objEquipo.ceq_icod_equipo);
                        cmd.Parameters.AddWithValue("@ceq_vnombre_equipo", objEquipo.ceq_vnombre_equipo);
                        cmd.Parameters.AddWithValue("@cvr_icod_version", objEquipo.cvr_icod_version);
                        cmd.Parameters.AddWithValue("@ceq_sfecha_actualizacion", objEquipo.ceq_sfecha_actualizacion);
                        cmd.Parameters.AddWithValue("@cep_vubicacion_actualizador", objEquipo.cep_vubicacion_actualizador);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal ControlEquipos Equipo_Obtner_Datos(string nombre, string idCpu)
        {
            ControlEquipos obj = new ControlEquipos();
            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ACT_EQUIPO_DATOS_OBTENER", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@cep_vid_cpu", idCpu);
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            obj.ceq_icod_equipo = Convert.ToInt32(reader["ceq_icod_equipo"]);
                            obj.ceq_vnombre_equipo = reader["ceq_vnombre_equipo"].ToString()!;
                            obj.cvr_icod_version = Convert.ToInt32(reader["cvr_icod_version"]);
                            var data = reader["ceq_sfecha_actualizacion"];
                            obj.ceq_sfecha_actualizacion = string.IsNullOrEmpty(data.ToString())? (DateTime?)null : Convert.ToDateTime(reader["ceq_sfecha_actualizacion"]);
                            obj.cvr_vversion = reader["cvr_vversion"].ToString()!;
                            data = reader["cvr_sfecha_version"];
                            obj.cvr_sfecha_version = string.IsNullOrEmpty(data.ToString()) ? (DateTime?)null: Convert.ToDateTime(reader["cvr_sfecha_version"]);
                            obj.cep_vubicacion_actualizador = reader["cep_vubicacion_actualizador"].ToString()!;
                            obj.cep_vid_cpu = reader["cep_vid_cpu"].ToString()!;
                            obj.cep_bflag_acceso = Convert.ToBoolean(reader["cep_bflag_acceso"]);
                            obj.cep_vubicacion_actualizador = reader["cep_vubicacion_actualizador"].ToString()!;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return obj;
        }

        internal List<ControlVersiones> Listar_Versiones()
        {
            List<ControlVersiones> lista = new List<ControlVersiones>();
            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("ACT_ACTUALIZACIONES_LISTAR", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            lista.Add(new ControlVersiones()
                            {
                                cvr_icod_version = Convert.ToInt32(reader["cvr_icod_version"]),
                                cvr_vversion = reader["cvr_vversion"].ToString()!,
                                cvr_sfecha_version = Convert.ToDateTime(reader["cvr_sfecha_version"]),
                                cvr_vurl = reader["cvr_vurl"].ToString()!,
                            }); ;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return lista;
        }

        internal List<Parametros> Parametros_Listar()
        {
            List<Parametros> lista = new List<Parametros>();
            try
            {
                using (SqlConnection cn = new SqlConnection(HelperConnection.conexion()))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("SIGAS_PARAMETRO_LISTAR", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = int.MaxValue;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {

                            lista.Add(new Parametros()
                            {

                                pm_nombre_empresa = reader["pm_nombre_empresa"].ToString()!,

                            }); ;

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return lista;
        }
    }
}

using System.Security.Cryptography;
using System.Text;

namespace SGIG.Negocio
{
    /// <summary>
    /// Hash de contraseñas con SHA256 (RNF#11). Las contraseñas nunca se guardan
    /// ni se comparan en texto plano: se guarda el hash y se comparan los hashes.
    /// </summary>
    public static class Hash
    {
        /// <summary>Calcula el SHA256 de una contraseña. Devuelve 32 bytes.</summary>
        public static byte[] Calcular(string contrasenia)
        {
            if (contrasenia is null)
            {
                throw new ArgumentNullException(nameof(contrasenia));
            }

            return SHA256.HashData(Encoding.UTF8.GetBytes(contrasenia));
        }

        /// <summary>
        /// Compara una contraseña en texto plano contra un hash guardado.
        /// Usa una comparación de tiempo fijo para no filtrar información por el
        /// tiempo que tarda en fallar.
        /// </summary>
        public static bool Coincide(string contrasenia, byte[] hashGuardado)
        {
            if (hashGuardado is null || hashGuardado.Length == 0)
            {
                return false;
            }

            return CryptographicOperations.FixedTimeEquals(Calcular(contrasenia), hashGuardado);
        }

        /// <summary>
        /// Representación hexadecimal del hash, para poder pegarlo en un script SQL
        /// (por ejemplo al sembrar el usuario administrador inicial).
        /// </summary>
        public static string ATextoHex(byte[] hash) => "0x" + Convert.ToHexString(hash);
    }
}

using System;
using System.Numerics;

namespace VeggieBack.Controllers {
    public class DiffieHellman {
        public int GenerateKeys(int info) {
            var g = new BigInteger(43);
            var p = new BigInteger(107);

            var a = new BigInteger(new Random().Next());
            var b = new BigInteger(info);

            var secretKey = (int)BigInteger.ModPow(g, b, p);
            var publicKey = (int)BigInteger.ModPow(g, a, p);
            return ObtainsKey(secretKey, publicKey);
        }
        public static int ObtainsKey(BigInteger privada, BigInteger publica) {
            return (int)BigInteger.ModPow(publica, privada, new BigInteger(107));
        }
    }
}

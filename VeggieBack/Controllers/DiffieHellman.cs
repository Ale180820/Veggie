using System;
using System.Numerics;

namespace VeggieBack.Controllers {
    public class DiffieHellman {

        int public_one = 0;
        int public_two = 0;

        public void GenerateKeys(int info) {
            var g = new BigInteger(43);
            var p = new BigInteger(107);

            var a = new BigInteger(new Random().Next());
            var b = new BigInteger(info);

            this.public_one = (int)BigInteger.ModPow(g, b, p);
            this.public_two = (int)BigInteger.ModPow(g, a, p);
        }

        public int getPrivateKey(BigInteger privada, BigInteger publica) {
            return (int)BigInteger.ModPow(publica, privada, new BigInteger(107));
        }

        public int getPublicOne(){
            return public_one;
        }

        public int getPublicTwo(){
            return public_two;
        }
    }
}

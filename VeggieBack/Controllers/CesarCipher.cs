using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VeggieBack.Controllers {
    public class CesarCipher {
        #region Dictionaries
        Dictionary<char, char> encryptionCode = new Dictionary<char, char>();
        //Cipher dictionary
        private void createKey(string privateKey) {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] key = privateKey.ToCharArray();
            int pos = 0;
            for (int i = 0; i < key.Length; i++) {
                encryptionCode.Add(alpha[i], key[i]);
                pos = i;
            }
            pos++;
            for (int i = 0; i < alpha.Length; i++) {
                if (!encryptionCode.ContainsValue(alpha[i])) {
                    encryptionCode.Add(alpha[pos], alpha[i]);
                    pos++;
                }
            }
        }
        //Decipher dictionary
        private void createDeKey(string privateKey) {
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] key = privateKey.ToCharArray();
            int pos = 0;
            for (int i = 0; i < key.Length; i++) {
                encryptionCode.Add(key[i], alpha[i]);
                pos = i;
            }
            pos++;
            for (int i = 0; i < alpha.Length; i++) {
                if(!encryptionCode.ContainsKey(alpha[i])) {
                    encryptionCode.Add(alpha[i], alpha[pos]);
                    pos++;
                }
            } 
        }
        #endregion

        #region Cipher password
        private string cipherPassword(string password) {
            string result = "";
            char[] line;
            line = password.ToCharArray();
            for (int i = 0; i < line.Length; i++) {
                if (encryptionCode.ContainsKey(line[i])) {
                    result += encryptionCode[line[i]];
                }
                else {
                    result += line[i];
                }
            }
            encryptionCode.Clear();
            return result;
        }
        #endregion

        #region Encryption
        public string Encryption(string password) {
            createKey("Vegi");
            string cipherPass;
            cipherPass = cipherPassword(password);
            return cipherPass;
        }
        #endregion
        #region Dencryption
        public string Dencryption(string password) {
            createDeKey("Vegi");
            string decipherPass;
            decipherPass = cipherPassword(password);
            return decipherPass;
        }
        #endregion 
    }
}

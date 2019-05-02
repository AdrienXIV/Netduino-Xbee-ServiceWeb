using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;
using System.Text;

namespace System.Diagnostics
{
    public enum DebuggerBrowsableState
    {
        Never = 0,
        Collapsed = 2,
        RootHidden = 3
    }
}

namespace NetduinoApplication2
{
    public class Program
    {
        private static byte[] msg;
        private static String donnee;
        private static bool compteur = false;
        private static OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
        private static Mfrc522 mfrc = new Mfrc522(SPI.SPI_module.SPI1, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D10);
        private static Uid idCarte; // pour récupérer la valeur retour avec la méthode ReadUid()

        protected static SerialPort sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        public static void Main()
        {
            // Request
            while (true)
            {
                mfrc.HaltTag(); //envoie la commande halt au capteur RFID pour éviter qu'il le lise pleins de fois
                if (mfrc.IsTagPresent()) //Verifie si il y a une carte devant le capteur
                {
                    led.Write(true);
                    idCarte = mfrc.ReadUid();
                    if (idCarte.GetHashCode().ToString().Length == 9) //si l'identifiant est complet lors de la lecture
                    {
                        donnee = idCarte.GetHashCode().ToString(); //enregistre ID
                        msg = Encoding.UTF8.GetBytes(donnee); // conserve dans une variable byte[] la chaîne de caractère après transformation en byte
                        compteur = true;
                    }
                }
                else
                {
                    led.Write(false);
                    Affiche(msg);
                    compteur = false;
                }
            }
        }
        private static void Affiche(byte[] data) //fonction qui affiche l'ID dans la console
        {
            if (compteur == true) //affichage des données quand "compteur" prend la valeur "true" au niveau de la détection
            {
                sp.Open(); // ouverture du port série
                Debug.Print(donnee);
                sp.Write(msg, 0, msg.Length); // envoie le message yop
                sp.Flush();
                sp.Close();
            }
        }
    }
}


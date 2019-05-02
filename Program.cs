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
        public static SerialPort sp;
        public static void Main()
        {
            var led = new OutputPort(Pins.ONBOARD_LED, false);
            var mfrc = new Mfrc522(SPI.SPI_module.SPI1, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D10);
            sp = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One); // création du port série
            sp.Open(); // ouverture du port série

            // Request
            while (true)
            {
                mfrc.HaltTag(); //envoie la commande halt au capteur RFID pour éviter qu'il le lise pleins de fois

                if(mfrc.IsTagPresent()) //Verifie si il y a une carte devant le capteur
                {
                    led.Write(true);
                    Uid idCarte; // pour récupérer la valeur retour avec la méthode ReadUid()
                    idCarte = mfrc.ReadUid();
                    Debug.Print(idCarte.GetHashCode() + "\n");
                    byte[] msg = Encoding.UTF8.GetBytes(idCarte.GetHashCode().ToString()); // conserve dans une variable byte[] la chaîne de caractère "yop" après transformation en byte
                    sp.Write(msg, 0, msg.Length); // envoie le message yop

                    sp.Flush();
                }
                else
                {
                    led.Write(false);
                }
            }
        }
    }

}


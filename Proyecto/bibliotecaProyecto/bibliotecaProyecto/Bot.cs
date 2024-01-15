using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot;
using System.Collections;
using System.IO;
using System.Threading;
using System.IO.Ports;

// Interfaz gráfica

// Biblioteca de clases
namespace bibliotecaProyecto
{
    public class Bot
    {
        //Listas
        private ArrayList usuarios = new ArrayList();

        //Arreglos
        private string[] leds = { "0", "0", "0", "0", "0" };

        private string estadoLeds, chatId;

        public ITelegramBotClient telebot;

        string mensajeRecibido;
        SerialPort puertoBot;
        public Bot(SerialPort puerto)
        {
            puertoBot = puerto;
        }

        public void prender()
        {
            Thread botHilo = new Thread(principal);
            botHilo.Start();
        }

        private void principal()
        {
            telebot = new TelegramBotClient("741700223:AAGj2P0HBrIQ12bV9veOu4XM177jTt5gmCc");
            usuarios.Add("518850937");
            // Eventos
            telebot.OnMessage += Comandos;
            telebot.StartReceiving();
        }

        public void agregarChat(string usuario)
        {
            if (usuarios.Contains(usuario)) { mandarMensaje("Ya estás agregado!", usuario); }
            else {
                usuarios.Add(usuario);
                mandarMensaje("Agregado!", usuario);
            }
        }
        public void eliminarChat(string usuario)
        {
            if (usuario.Contains(usuario))
            {
                mandarMensaje("Eliminando..");
                usuarios.Remove(usuario);
            }
            else { mandarMensaje("Tu no estabas agregado..", usuario); }
            
        }

        private void Comandos(object sender, MessageEventArgs e)
        {
            mensajeRecibido = e.Message.Text;
            chatId = e.Message.Chat.Id.ToString();
            if (mensajeRecibido == null) { return; }
            string[] mensajeCompleto = mensajeRecibido.Split(' ');
            switch (mensajeCompleto[0])
            {
                case "/saluda":
                    saludar(chatId);
                    break;
                case "/mandar":
                    try
                    {
                        mandarArchivo(mensajeCompleto[1], chatId);
                    }
                    catch
                    {
                        mandarMensaje("No entendí..", chatId);
                    }
                    break;
                case "/lectura":
                    try
                    {
                        mandarLectura(mensajeCompleto[1], chatId);
                    }
                    catch
                    {
                        mandarMensaje("No entendí..", chatId);
                    }
                    break;
                case "/prender":
                    try
                    {
                        prender(mensajeCompleto[1], chatId);
                    }
                    catch
                    {
                        mandarMensaje("No entendí..", chatId);
                    }
                    break;
                case "/apagar":
                    try
                    {
                        apagar(mensajeCompleto[1], chatId);
                    }
                    catch
                    {
                        mandarMensaje("No entendí..", chatId);
                    }
                    break;
                case "/id":
                    mandarID(chatId,chatId);
                    break;
                case "/agregarme":
                    agregarChat(chatId);
                    break;
                case "/eliminarme":
                    eliminarChat(chatId);
                    break;
                case "/agregar":
                    try
                    {
                        agregarChat(mensajeCompleto[1]);
                    }
                    catch
                    {
                        mandarMensaje("No endendí..");
                    }
                    break;
                case "/eliminar":
                    try
                    {
                        eliminarChat(mensajeCompleto[1]);
                    }
                    catch
                    {
                        mandarMensaje("No endendí..");
                    }
                    break;
                default:
                    echo(mensajeRecibido, chatId);
                    break;
            }
        }

        // Sobrecarga
        public void mandarMensaje(string texto)
        {
            try
            {
                foreach (string id in usuarios)
                {
                    telebot.SendTextMessageAsync(
                                chatId: id,
                                text: texto);
                }
            }
            catch { }
        }

        private void mandarMensaje(string texto, string chat)
        {
            try
            { 
                telebot.SendTextMessageAsync(
                            chatId: chat,
                            text: texto);
            }
            catch { }
        }

        private void saludar(string chat)
        {
            mandarMensaje("Hola!!",chat);            
        }

        // Archivos
        private void mandarArchivo(string path,string chat)
        {
            try
            {
                StreamReader lector = new StreamReader(path);
                telebot.SendTextMessageAsync(chat, lector.ReadToEnd());
                lector.Close();
            }
            catch
            {
                mandarMensaje("No encontré el archivo :z",chat);
            }
        }

        private void mandarID(string id, string chat)
        {
            mandarMensaje("Tu id es: "+chat, chat);
        }


        private void mandarLectura(string lect,string chat)
        { 
            switch (lect)
            {
                // Métodos static
                case "proxi":
                    mandarMensaje(Lecturas.prox[Lecturas.prox.Length - 1].ToString(), chat);
                    break;
                case "pot":
                    mandarMensaje(Lecturas.pot[Lecturas.pot.Length - 1].ToString(), chat);
                    break;
                case "luz":
                    mandarMensaje(Lecturas.ilum[Lecturas.ilum.Length - 1].ToString(), chat);
                    break;
                default:
                    mandarMensaje("No tenemos esa lectura..", chat);
                    break;
            }
        }

        // comunicación serial
        private void prender(string led, string chat)
        {
            try
            {
                switch (led)
                {
                    case "1":
                        estadoLeds = "";
                        leds[0] = "1";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo foco "+led,chat);
                        break;
                    case "2":
                        estadoLeds = "";
                        leds[1] = "1";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo foco " + led,chat);
                        break;
                    case "3":
                        estadoLeds = "";
                        leds[2] = "1";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo foco " + led,chat);
                        break;
                    case "4":
                        estadoLeds = "";
                        leds[3] = "1";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo foco " + led,chat);
                        break;
                    case "5":
                        estadoLeds = "";
                        leds[4] = "1";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo foco " + led,chat);
                        break;
                    case "todos":
                        estadoLeds = "";
                        for (int i = 0; i < leds.Length; i++) { leds[i] = "1"; }
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Prendiendo todos los focos",chat);
                        break;
                    default:
                        mandarMensaje("No se de que foco hablas..",chat);
                        break;
                }
            }
            catch
            {
                mandarMensaje("No me es posible prender la luz :c",chat);
            }
        }

        public void prenderTodos()
        {
            try
            {
                puertoBot.Write("11111");
                mandarMensaje("Hay poca luz, voy a prender todos los focos");
            }
            catch
            {
                mandarMensaje("No me es posible prender la luz :c");
            }
        }
        public void apagarTodo()
        {
            try
            {
                foreach (string elemLed in leds)
                    estadoLeds += elemLed;
                puertoBot.Write(estadoLeds);
                mandarMensaje("Hay suficiente luz, voy a apagar los focos");
            }
            catch
            {
                mandarMensaje("No me es posible apagar la luz :c");
            }
        }

        private void apagar(string led, string chat)
        {
            try
            {
                switch (led)
                {
                    case "1":
                        estadoLeds = "";
                        leds[0] = "0";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando foco " + led, chat);
                        break;
                    case "2":
                        estadoLeds = "";
                        leds[1] = "0";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando foco " + led, chat);
                        break;
                    case "3":
                        estadoLeds = "";
                        leds[2] = "0";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando foco " + led, chat);
                        break;
                    case "4":
                        estadoLeds = "";
                        leds[3] = "0";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando foco " + led, chat);
                        break;
                    case "5":
                        estadoLeds = "";
                        leds[4] = "0";
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando foco " + led, chat);
                        break;
                    case "todos":
                        estadoLeds = "";
                        for (int i = 0; i < leds.Length; i++) { leds[i] = "0"; }
                        foreach (string elemLed in leds)
                            estadoLeds += elemLed;
                        puertoBot.Write(estadoLeds);
                        mandarMensaje("Apagando todos los focos", chat);
                        break;
                    default:
                        mandarMensaje("No se de que foco hablas..", chat);
                        break;
                }
            }
            catch
            {
                mandarMensaje("No me es posible apagar la luz :c", chat);
            }
        }
        private void echo(string mensaje,string chat)
        {
            mandarMensaje(mensaje, chat);
        }

    }
}

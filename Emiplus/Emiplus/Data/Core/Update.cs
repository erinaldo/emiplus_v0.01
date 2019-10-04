﻿using System.Net;

namespace Emiplus.Data.Core
{
    class Update
    {
        public static bool AtualizacaoDisponivel { get; set; }

        /// <summary>
        /// Pega versão recente do app na web
        /// </summary>
        public string GetVersionWebTxt()
        {
            using (WebClient client = new WebClient())
            {
                string version = client.DownloadString("https://emiplus.com.br/version/version.txt");
                return version;
            }
        }

        /// <summary>
        /// Verifica versão da web com a versão do app e atualiza o INI para disponibilizar a versão mais recente
        /// </summary>
        public void CheckUpdate()
        {
            if (GetVersionWebTxt() != IniFile.Read("Version", "APP"))
            {
                AtualizacaoDisponivel = true;
                IniFile.Write("Update", "true", "APP");
                return;
            }

            IniFile.Write("Update", "false", "APP");
            AtualizacaoDisponivel = false;
        }
        
        /// <summary>
        /// Verifica se existe as KEYs principais de configuração no arquivo INI, e adiciona caso não exista
        /// </summary>
        public void CheckIni()
        {
            if (!IniFile.KeyExists("none", "DEFAULT"))
                IniFile.Write("none", "none", "DEFAULT");

            if (!IniFile.KeyExists("Version", "APP"))
                IniFile.Write("Version", "1.0.0", "APP");

            if (!IniFile.KeyExists("URL_Ajuda", "APP"))
                IniFile.Write("URL_Ajuda", "http://ajuda.emiplus.com.br", "APP");

            if (!IniFile.KeyExists("URL_Base", "APP"))
                IniFile.Write("URL_Base", "http://www.emiplus.com.br", "APP");

            if (!IniFile.KeyExists("Update", "APP"))
                IniFile.Write("Update", "true", "APP");

            if (!IniFile.KeyExists("Path", "LOCAL"))
                IniFile.Write("Path", @"C:\Emiplus", "LOCAL");

            if (!IniFile.KeyExists("PathDatabase", "LOCAL"))
                IniFile.Write("PathDatabase", @"C:\Emiplus", "LOCAL");
            
            if (!IniFile.KeyExists("dev", "DEV"))
                IniFile.Write("dev", "false", "DEV");
        }
    }
}
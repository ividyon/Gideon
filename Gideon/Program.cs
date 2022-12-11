using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using SoulsFormats;

namespace Gideon
{
    internal class Program
    {
        private enum RunMode
        {
            Prompts,
            ConsolePrompts,
            Arguments
        }

        [STAThread]
        public static void Main(string[] args)
        {
            RunMode mode = RunMode.Prompts;

            string ddsPath = null;
            string bhdPath = null;

            if (args.Contains("-c") || args.Contains("--console"))
            {
                mode = RunMode.ConsolePrompts;
            }
            else if (args.Length == 2)
            {
                mode = RunMode.Arguments;
            }

            if (mode != RunMode.Prompts)
            {
                Console.WriteLine("You've received the wisdom of the Two Fingers, have you not?");
                Console.WriteLine("I bid you welcome, as a true member of the Roundtable.");
                Console.WriteLine("I am known as Gideon Ofnir.");
            }

            switch (mode)
            {
                case RunMode.Prompts:
                    using (var dirDialog = new CommonOpenFileDialog())
                    {
                        dirDialog.IsFolderPicker = true;
                        dirDialog.Title = "Select folder containing MENU_Knowledge DDS files...";
                        CommonFileDialogResult result = dirDialog.ShowDialog();

                        if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dirDialog.FileName))
                        {
                            ddsPath = dirDialog.FileName;
                        }
                        else if (result == CommonFileDialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    using (var fileDialog = new OpenFileDialog())
                    {
                        fileDialog.Title = "Select TPFBHD file...";
                        fileDialog.Filter = "TPFBHD files (*.tpfbhd)|*.tpfbhd|All files (*.*)|*.*";
                        fileDialog.Multiselect = false;

                        DialogResult result = fileDialog.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fileDialog.FileName))
                        {
                            bhdPath = fileDialog.FileName;
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            return;
                        }
                    }

                    break;
                case RunMode.ConsolePrompts:
                    Console.Write("Enter path to folder containing DDS files:");
                    ddsPath = Console.ReadLine();
                    Console.Write("Enter path to DCX file to embed the DDS files in:");
                    bhdPath = Console.ReadLine();
                    break;
                case RunMode.Arguments:
                    ddsPath = args[0];
                    bhdPath = args[1];
                    break;
            }

            if (ddsPath == null || !Directory.Exists(ddsPath))
            {
                switch (mode)
                {
                    case RunMode.Prompts:
                        MessageBox.Show(
                            "Please provide a valid path to a folder containing DDS files with the right naming (MENU_Knowledge_*.dds).",
                            "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        Console.Write(
                            "Please provide a valid path to a folder containing DDS files with the right naming (MENU_Knowledge_*.dds).");
                        break;
                }

                return;
            }

            if (bhdPath == null || !File.Exists(bhdPath))
            {
                switch (mode)
                {
                    case RunMode.Prompts:
                        MessageBox.Show("Please provide a valid path to a TPFBHD archive file.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        Console.Write("Please provide a valid path to a TPFBHD archive file.");
                        break;
                }

                return;
            }

            string[] ddsFilePaths = Directory.GetFiles(ddsPath, "MENU_Knowledge_*.dds");

            string bdtPath = $@"{Path.GetDirectoryName(bhdPath)}\{Path.GetFileNameWithoutExtension(bhdPath)}.tpfbdt";
            BXF4 bxf = BXF4.Read(bhdPath, bdtPath);
            int nextId = bxf.Files.OrderByDescending(item => item.ID).First().ID + 1;
            int fileWrites = 0;
            int replaces = 0;
            int newFiles = 0;
            foreach (string ddsFilePath in ddsFilePaths)
            {
                string textureName = Path.GetFileNameWithoutExtension(ddsFilePath);
                byte[] dds = File.ReadAllBytes(ddsFilePath);
                TPF tpf = new TPF();
                TPF.Texture texture = new TPF.Texture();
                texture.Name = textureName;
                texture.Bytes = dds;
                tpf.Textures.Add(texture);
                byte[] dcx = tpf.Write(DCX.Type.DCX_KRAK);

                BinderFile existingFile = bxf.Files.Find(file => file.Name == $@"00_Solo\{textureName}.tpf.dcx");
                if (existingFile != null)
                {
                    existingFile.Bytes = dcx;
                    replaces++;
                }
                else
                {
                    bxf.Files.Add(
                        new BinderFile(Binder.FileFlags.Flag1, nextId, $@"00_Solo\{textureName}.tpf.dcx", dcx));
                    newFiles++;
                }

                nextId++;
                fileWrites++;
            }

            bxf.Write(bhdPath, bdtPath);

            string message =
                $"Successfully wrote {fileWrites} files ({newFiles} new files, {replaces} existing files replaced)";

            switch (mode)
            {
                case RunMode.Prompts:
                    MessageBox.Show(message, "You are a true fellow", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    break;
                default:
                    Console.WriteLine(message);
                    break;
            }
        }
    }
}
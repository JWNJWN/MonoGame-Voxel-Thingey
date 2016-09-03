using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace VoxEngine.Helpers
{
    public static class FileHelper
    {
        public static FileStream CreateGameContentFile(string relativeFilename, bool createNew)
        {
            string fullPath = Path.Combine("Contents/", relativeFilename);
            return File.Open(fullPath, createNew ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }

        public static FileStream LoadGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine("Contents/", relativeFilename);
            if (!File.Exists(fullPath))
                return null;
            else
                return File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public static FileStream SaveGameContentFile(string relativeFilename)
        {
            string fullPath = Path.Combine("Contents/", relativeFilename);
            return File.Open(fullPath, FileMode.Create, FileAccess.Write);
        }

        public static FileStream OpenFileForCurrentPlayer(string filename, FileMode mode, FileAccess access)
        {
            string fullFileName = Path.Combine("Contents/", filename);
            return new FileStream(fullFileName, mode, access, FileShare.ReadWrite);
        }

        static public string[] GetLines(string filename)
        {
            try
            {
                StreamReader reader = new StreamReader(
                    new FileStream(filename, FileMode.Open, FileAccess.Read),
                    System.Text.Encoding.UTF8);
                List<string> lines = new List<string>();
                do
                {
                    lines.Add(reader.ReadLine());
                } while (reader.Peek() > -1);
                reader.Close();
                return lines.ToArray();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        public static void WriteVector3(BinaryWriter writer, Vector3 vector)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
        }
        
        public static void WriteVector4(BinaryWriter writer, Vector4 vector)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(vector.X);
            writer.Write(vector.Y);
            writer.Write(vector.Z);
            writer.Write(vector.W);
        }

        public static void WriteMatrix(BinaryWriter writer, Matrix matrix)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(matrix.M11);
            writer.Write(matrix.M12);
            writer.Write(matrix.M13);
            writer.Write(matrix.M14);
            writer.Write(matrix.M21);
            writer.Write(matrix.M22);
            writer.Write(matrix.M23);
            writer.Write(matrix.M24);
            writer.Write(matrix.M31);
            writer.Write(matrix.M32);
            writer.Write(matrix.M33);
            writer.Write(matrix.M34);
            writer.Write(matrix.M41);
            writer.Write(matrix.M42);
            writer.Write(matrix.M43);
            writer.Write(matrix.M44);
        }
        
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }
        
        public static Vector4 ReadVector4(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector4(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        public static Matrix ReadMatrix(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Matrix(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }
    }
}

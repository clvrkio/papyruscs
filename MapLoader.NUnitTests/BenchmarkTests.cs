﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Maploader.Renderer;
using Maploader.Renderer.Heightmap;
using Maploader.Renderer.Texture;
using NUnit.Framework;

namespace MapLoader.NUnitTests
{
    [TestFixture]
    class BenchmarkTests
    {
        [Test]
        public void Open()
        {
            Console.WriteLine("hello world");
            var dut = new Maploader.World.World();
            dut.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "benchmark", "world", "db"));

           
            foreach (var d in dut.Keys.Where(x => x.Length >= 9 & x[8] == 45 ))
            {
                //Console.WriteLine(string.Join(" ", d.Select(e => $"{e:d3}")));
                //Console.WriteLine(dut.GetData(d).Length);
            }

            //dut.Close();

            Assert.Pass();
        }

        [Test]
        [Ignore("debugging")]
        public void TestRender()
        {
            var dut = new Maploader.World.World();
            dut.Open(@"C:\papyruscs\homeworld\db");
            int chunkRadius = 2;
            int centerOffsetX = 12; //65;
            int centerOffsetZ = -55; //65;
            string filename = "testrender.png";

            RenderMap(chunkRadius, dut, centerOffsetX, centerOffsetZ, filename);
        }

        [Test]
        public void BenchmarkRender()
        {
            var dut = new Maploader.World.World();
            dut.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "benchmark", "world", "db"));
            int chunkRadius = 1;
            int centerOffsetX = 1; //65;
            int centerOffsetZ = 1; //65;
            string filename = "benchmark.png";

            RenderMap(chunkRadius, dut, centerOffsetX, centerOffsetZ, filename);
        }

        private static void RenderMap(int chunkRadius, Maploader.World.World dut, int centerOffsetX, int centerOffsetZ, string filename)
        {
            var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"textures",
                "terrain_texture.json"));
            var ts = new TerrainTextureJsonParser(json, "");
            var textures = ts.Textures;
            var finder = new TextureFinder(textures, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "textures"));
            finder.Debug = false;

            var b = new Bitmap(16 * 16 * (2 * chunkRadius + 1), 16 * 16 * (2 * chunkRadius + 1));
            var g = Graphics.FromImage(b);

            var render = new ChunkRenderer(finder, new RenderSettings(){ YMax = 40});

            //Parallel.For(-chunkRadius, chunkRadius + 1,new ParallelOptions(){MaxDegreeOfParallelism = 8} , dx =>
            for (int dz = -chunkRadius; dz <= chunkRadius; dz++)
            {
                for (int dx = -chunkRadius; dx <= chunkRadius; dx++)
                {
                    var c = dut.GetChunk(dx + centerOffsetX, dz + centerOffsetZ);
                    if (c != null)
                    {
                        render.RenderChunk(b, c, g, (chunkRadius + dx) * 256, (chunkRadius + dz) * 256);
                    }
                }
            }

            ;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            b.Save(path);
            Console.WriteLine(path);
            dut.Close();
        }

        [Test]
        public void Uint64Test()
        {
            int x = 1;
            int z = 1;
            unchecked
            {
                var k = (UInt64) (
                    ((UInt64) (x) << 32) |
                    ((UInt64) (z) & 0xFFFFFFFF)
                );

                Console.WriteLine("{0:x8}", k);
                Assert.AreEqual(k, 0x100000001);
            }

        }

        [Test]
        public void BrillouinFkt()
        {
            var dut = new Brillouin(10000);

        }
    }
}

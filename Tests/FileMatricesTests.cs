﻿using Domain.Matrices;
using Domain.MatrixMultiplication;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Tests
{
    public class FileMatricesTests
    {
        private const string PathMatrixA = "..\\..\\..\\..\\matA.txt";
        private const string PathMatrixB = "..\\..\\..\\..\\matB.txt";
        private const string MD5HashResultMatrix = "c4d56edebadb13b0cdf98429a8e54414";
        private const string MD5HashMatrixA = "c06ab7d1d97602cdca927504fcf9b997";
        private const string MD5HashMatrixB = "3428d1ca214241a897c4370973357df9";

        public FileMatricesTests()
        {
            MatrixA = MatrixReader.Resolve(PathMatrixA);
            MatrixB = MatrixReader.Resolve(PathMatrixB);
        }

        public Matrix MatrixA { get;}
        public Matrix MatrixB { get;}

        [Test]
        public void ShouldReadValidMatrix()
        {
            MatrixA.X.Should().Be(4096);
            MatrixB.Y.Should().Be(4096);

            MatrixA.X.Should().Be(4096);
            MatrixB.Y.Should().Be(4096);
        }

        [Test]
        public void ShouldMultiplyFileMatricesSingleThreaded()
        {
            var multiplicator = new LocalMultiplicator(MatrixA, MatrixB);

            var resultMatrix = multiplicator.MultiplySingleThreaded();
            resultMatrix.X.Should().Be(MatrixA.X);
            resultMatrix.Y.Should().Be(MatrixB.Y);
            GetFileMD5Hash(resultMatrix.SaveFile()).Should().Be(MD5HashResultMatrix);
        }

        [Test]
        public void ShouldMultiplyFileMatricesMultiThreaded()
        {
            var multiplicator = new LocalMultiplicator(MatrixA, MatrixB);

            var resultMatrix = multiplicator.MultiplyMultiThreaded();
            resultMatrix.X.Should().Be(MatrixA.X);
            resultMatrix.Y.Should().Be(MatrixB.Y);
            GetFileMD5Hash(resultMatrix.SaveFile()).Should().Be(MD5HashResultMatrix);
        }

        [Test]
        [TestCase(PathMatrixA, MD5HashMatrixA)]
        [TestCase(PathMatrixB, MD5HashMatrixB)]
        public void ShouldVerifyMD5Hash(string path, string expectedHash)
        {
            GetFileMD5Hash(path).Should().Be(expectedHash);
        }

        public static string GetFileMD5Hash(string path) 
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var bytes = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
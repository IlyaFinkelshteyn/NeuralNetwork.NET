﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNetworkNET.Cuda.Helpers;
using NeuralNetworkNET.Helpers;
using NeuralNetworkNET.Networks.Activations;
using NeuralNetworkNET.Networks.Cost;

namespace NeuralNetworkNET.Cuda.Unit
{
    /// <summary>
    /// Test class for the <see cref="MatrixCudaExtensionsTest"/> class
    /// </summary>
    [TestClass]
    [TestCategory(nameof(MatrixCudaExtensionsTest))]
    [SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
    public class MatrixCudaExtensionsTest
    {
        [TestMethod]
        public void Transpose()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(7, 3),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(25, 180),
                m3 = ThreadSafeRandom.NextGlorotNormalMatrix(1428, 3811);
            Assert.IsTrue(MatrixExtensions.Transpose(m1).ContentEquals(MatrixGpuExtensions.Transpose(m1), 1e-4f));
            Assert.IsTrue(MatrixExtensions.Transpose(m2).ContentEquals(MatrixGpuExtensions.Transpose(m2), 1e-4f));
            Assert.IsTrue(MatrixExtensions.Transpose(m3).ContentEquals(MatrixGpuExtensions.Transpose(m3), 1e-4f));
        }

        [TestMethod]
        public void Multiply()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(7, 3),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(3, 4),
                check = MatrixExtensions.Multiply(m1, m2),
                test = MatrixGpuExtensions.Multiply(m1, m2);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(1500, 800);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 40);
            check = MatrixExtensions.Multiply(m1, m2);
            test = MatrixGpuExtensions.Multiply(m1, m2);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void TransposeAndMultiply()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(5, 13),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(5, 4),
                check = MatrixExtensions.Multiply(MatrixExtensions.Transpose(m1), m2),
                test = MatrixGpuExtensions.TransposeAndMultiply(m1, m2);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 1500);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 40);
            check = MatrixExtensions.Multiply(MatrixGpuExtensions.Transpose(m1), m2);
            test = MatrixGpuExtensions.TransposeAndMultiply(m1, m2);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void MultiplyWithSum()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(13, 5),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(5, 4);
            float[] v = ThreadSafeRandom.NextGaussianVector(4);
            float[,] check = MatrixExtensions.MultiplyWithSum(m1, m2, v);
            float[,] test = MatrixGpuExtensions.MultiplyWithSum(m1, m2, v);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(1500, 800);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 40);
            v = ThreadSafeRandom.NextGaussianVector(40);
            check = MatrixExtensions.MultiplyWithSum(m1, m2, v);
            test = MatrixGpuExtensions.MultiplyWithSum(m1, m2, v);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void MultiplyWithSumAndActivation()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(13, 5),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(5, 4);
            float[] v = ThreadSafeRandom.NextGaussianVector(4);
            float[,] check = MatrixExtensions.MultiplyWithSumAndActivation(m1, m2, v, ActivationFunctions.Sigmoid);
            float[,] test = MatrixGpuExtensions.MultiplyWithSumAndActivation(m1, m2, v, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(1500, 800);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 40);
            v = ThreadSafeRandom.NextGaussianVector(40);
            check = MatrixExtensions.MultiplyWithSumAndActivation(m1, m2, v, ActivationFunctions.Sigmoid);
            test = MatrixGpuExtensions.MultiplyWithSumAndActivation(m1, m2, v, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void MultiplyAndActivation()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(7, 3),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(3, 4),
                check = MatrixExtensions.MultiplyAndActivation(m1, m2, ActivationFunctions.Sigmoid);
            float[,] test = MatrixGpuExtensions.MultiplyAndActivation(m1, m2, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(1500, 800);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(800, 40);
            check = MatrixExtensions.MultiplyAndActivation(m1, m2, ActivationFunctions.Sigmoid);
            test = MatrixGpuExtensions.MultiplyAndActivation(m1, m2, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void Activation()
        {
            float[,]
                m = ThreadSafeRandom.NextGlorotNormalMatrix(20, 35),
                check = MatrixExtensions.Activation(m, ActivationFunctions.Sigmoid),
                test = MatrixGpuExtensions.Activation(m, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
            m = ThreadSafeRandom.NextGlorotNormalMatrix(1500, 800);
            check = MatrixExtensions.Activation(m, ActivationFunctions.Sigmoid);
            test = MatrixGpuExtensions.Activation(m, ActivationFunctions.Sigmoid);
            Assert.IsTrue(test.ContentEquals(check, 1e-4f));
        }

        [TestMethod]
        public void InPlaceSubtractAndHadamardProductWithActivationPrime()
        {
            float[,]
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                m3 = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                backup = new float[10, 10];
            Buffer.BlockCopy(m1, 0, backup, 0, sizeof(float) * m1.Length);
            CostFunctions.QuadraticCostPrime(backup, m2, m3, ActivationFunctions.SigmoidPrime);
            CostFunctions.QuadraticCostPrime(m1, m2, m3, ActivationFunctions.SigmoidPrime);
            Assert.IsTrue(m1.ContentEquals(backup, 1e-4f));
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            m3 = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            backup = new float[200, 200];
            Buffer.BlockCopy(m1, 0, backup, 0, sizeof(float) * m1.Length);
            CostFunctions.QuadraticCostPrime(backup, m2, m3, ActivationFunctions.SigmoidPrime);
            CostFunctions.QuadraticCostPrime(m1, m2, m3, ActivationFunctions.SigmoidPrime);
            Assert.IsTrue(m1.ContentEquals(backup, 1e-4f));
        }

        [TestMethod]
        public void MultiplyAndInPlaceActivationPrimeAndHadamardProduct()
        {
            float[,]
                wt = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                m1 = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                m2 = ThreadSafeRandom.NextGlorotNormalMatrix(10, 10),
                backup = new float[10, 10];
            Buffer.BlockCopy(m1, 0, backup, 0, sizeof(float) * m1.Length);
            MatrixExtensions.InPlaceMultiplyAndHadamardProductWithAcrivationPrime(backup, m2, wt, ActivationFunctions.SigmoidPrime);
            MatrixGpuExtensions.MultiplyAndInPlaceActivationPrimeAndHadamardProduct(m1, m2, wt, ActivationFunctions.SigmoidPrime);
            Assert.IsTrue(m1.ContentEquals(backup, 1e-4f));
            wt = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            m1 = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            m2 = ThreadSafeRandom.NextGlorotNormalMatrix(200, 200);
            backup = new float[200, 200];
            Buffer.BlockCopy(m1, 0, backup, 0, sizeof(float) * m1.Length);
            MatrixExtensions.InPlaceMultiplyAndHadamardProductWithAcrivationPrime(backup, m2, wt, ActivationFunctions.SigmoidPrime);
            MatrixGpuExtensions.MultiplyAndInPlaceActivationPrimeAndHadamardProduct(m1, m2, wt, ActivationFunctions.SigmoidPrime);
            Assert.IsTrue(m1.ContentEquals(backup, 1e-4f));
        }
    }
}
﻿using Alea;
using Alea.cuDNN;
using Alea.Parallel;
using JetBrains.Annotations;
using NeuralNetworkNET.Networks.Activations.Delegates;

namespace NeuralNetworkNET.cuDNN
{
    /// <summary>
    /// A static class with some extensions for the <see cref="Dnn"/> class
    /// </summary>
    public static class CuDnnExtensions
    {
        #region Activation

        /// <summary>
        /// Executes the input activation function on the target memory area. The input and output pointers can be the same, if needed
        /// </summary>
        /// <param name="dnn">The current <see cref="Dnn"/> instance being used</param>
        /// <param name="n">The number of samples in the input tensor</param>
        /// <param name="w">The size of each sample to process</param>
        /// <param name="x">A pointer to the input memory area</param>
        /// <param name="y">The output memory area</param>
        /// <param name="f">The activation function to use</param>
        public static void ActivationForward([NotNull] this Dnn dnn, int n, int w, deviceptr<float> x, deviceptr<float> y, [NotNull] ActivationFunction f)
        {
            void Kernel(int i)
            {
                int offset = i * w;
                for (int j = 0; j < w; j++)
                {
                    int target = offset + j;
                    y[target] = f(x[target]);
                }
            }
            dnn.Gpu.For(0, n, Kernel);
        }

        /// <summary>
        /// Executes the backward activation function on the target memory area, with the given error delta
        /// </summary>
        /// <param name="dnn">The current <see cref="Dnn"/> instance being used</param>
        /// <param name="n">The number of samples in the input tensor</param>
        /// <param name="w">The size of each sample to process</param>
        /// <param name="z">A pointer to the input memory area</param>
        /// <param name="delta">The delta memory area</param>
        /// <param name="f">The activation function to use</param>
        public static void ActivationBackward([NotNull] this Dnn dnn, int n, int w, deviceptr<float> z, deviceptr<float> delta, [NotNull] ActivationFunction f)
        {
            void Kernel(int i)
            {
                int offset = i * w;
                for (int j = 0; j < w; j++)
                {
                    int target = offset + j;
                    z[target] = f(z[target]) * delta[target];
                }
            }
            dnn.Gpu.For(0, n, Kernel);
        }

        #endregion

        #region Fully connected

        /// <summary>
        /// Executes the forward pass on a fully connected layer
        /// </summary>
        /// <param name="dnn">The current <see cref="Dnn"/> instance being used</param>
        /// <param name="n">The number of samples in the input tensor</param>
        /// <param name="l">The size of each input sample to process</param>
        /// <param name="k">The number of output features for each sample</param>
        /// <param name="x">A pointer to the input memory area</param>
        /// <param name="w">A pointer to the layer weights</param>
        /// <param name="b">A pointer to the network biases</param>
        /// <param name="y">A pointer to the output memory area</param>
        public static void FullyConnectedForward([NotNull] this Dnn dnn, int n, int l, int k, deviceptr<float> x, deviceptr<float> w, deviceptr<float> b, deviceptr<float> y)
        {
            void Kernel(int index)
            {
                // Calculate the current indexes
                int
                    i = index / k,
                    j = index % k;

                // Perform the multiplication
                float sum = 0;
                int pm1_offset = i * l;
                for (int z = 0; z < l; z++)
                {
                    sum += x[pm1_offset + z] * w[z * k + j];
                }
                y[i * k + j] = sum + b[j]; // Sum the input vector to each component
            }
            dnn.Gpu.For(0, n * k, Kernel);
        }

        /// <summary>
        /// Executes the backward pass on a fully connected layer
        /// </summary>
        /// <param name="dnn">The current <see cref="Dnn"/> instance being used</param>
        /// <param name="n">The number of samples in the input tensor</param>
        /// <param name="k">The number of output features for each sample</param>
        /// <param name="l">The size of each input error delta to process</param>
        /// <param name="z">A pointer to the activity on the previous layer</param>
        /// <param name="dy">A pointer to the output error delta</param>
        /// <param name="w">A pointer to the layer weights</param>
        /// <param name="f_">The derivative of the activation function of the previous layer</param>
        public static void FullyConnectedBackwardData([NotNull] this Dnn dnn, int n, int k, int l, deviceptr<float> z, deviceptr<float> dy, deviceptr<float> w, [NotNull] ActivationFunction f_)
        {
            void Kernel(int index)
            {
                // Calculate the current indexes
                int
                    i = index / k,
                    j = index % k;

                // Perform the multiplication (the second matrix is transposed while being processed)
                float sum = 0;
                int
                    dy_offset = i * l,
                    w_offset = j * l;
                for (int iter = 0; iter < l; iter++)
                {
                    sum += dy[dy_offset + iter] * w[w_offset + iter];
                }

                // Activation
                int z_offset = i * k + j;
                z[z_offset] = f_(z[z_offset]) * sum;
            }
            dnn.Gpu.For(0, n * k, Kernel);
        }

        /// <summary>
        /// Executes the backward pass on a fully connected layer to calculate the gradient with respect to the weights
        /// </summary>
        /// <param name="dnn">The current <see cref="Dnn"/> instance being used</param>
        /// <param name="n">The number of samples in the input tensor</param>
        /// <param name="l">The number of features for each input sample</param>
        /// <param name="k">The number of features in the output error delta</param>
        /// <param name="x">A pointer to the input tensor</param>
        /// <param name="dy">A pointer to the output error delta</param>
        /// <param name="dw">A pointer to a memory area to use to saave the computed weights gradient</param>
        public static void FullyConnectedBackwardFilter([NotNull] this Dnn dnn, int n, int l, int k, deviceptr<float> x, deviceptr<float> dy, deviceptr<float> dw)
        {
            void Kernel(int index)
            {
                // Calculate the current indexes
                int
                    i = index / k,
                    j = index % k;

                // Perform the multiplication
                float sum = 0;
                for (int iter = 0; iter < n; iter++)
                {
                    sum += x[iter * l + i] * dy[iter * k + j];
                }
                dw[i * k + j] = sum;
            }
            dnn.Gpu.For(0, l * k, Kernel);
        }

        #endregion
    }
}

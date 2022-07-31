import pandas as pd
import numpy as np
import math
import statistics
import scipy.stats
import sklearn.feature_selection
import scipy.fft
from sklearn import preprocessing

import tensorflow as tf

from tensorflow.keras import layers, models

# paper on methodology: https://link.springer.com/chapter/10.1007/978-3-030-29933-0_37

def get_stats(num_cols, channel_data):
    num_label_rows = 2 # the 2 rows at the end aren't important -- used for label info

    # df.iloc[i] will grab row i
    # df[i] grabs column i

    # 1). find mean
    # 2). find standard deviation
    # 3). find sample skewness & kurtosis
    means = np.zeros((num_cols - num_label_rows))
    std_dev = np.zeros((num_cols - num_label_rows))
    num_channels = num_cols - num_label_rows

    skewness = np.zeros((num_cols - num_label_rows))
    kurtosis = np.zeros((num_cols - num_label_rows))

    for channel in range(num_channels):
        means[channel] = statistics.mean(channel_data[channel])
        std_dev[channel] = statistics.stdev(channel_data[channel])
        skewness[channel] = scipy.stats.skew(channel_data[channel])
        kurtosis[channel] = scipy.stats.kurtosis(channel_data[channel])

    # 4). gather the covariance of each pair & variance of elements
    covariances = []
    num_combs = math.comb(num_channels, 2)
    matrices = np.cov(channel_data)
    for i in range(matrices.shape[0]):
        for j in range(i, matrices.shape[1]):
            covariances.append(matrices[i][j])

    # 5). find eigenvalues of covariance matrix 
    eigenvalues_of_cov = np.linalg.eig(matrices)[0]

    # 6). find 10 most energetic components of each channel
    ffts = scipy.fft.fft(channel_data)
    ten_maxes = np.empty((num_channels, 10))
    # each channel gets a row & each row has 10 entries
    for i in range(num_channels):
        # grab 10 largest magnitudes for each element
        ind = np.argpartition(ffts[i], -10)[-10:]
        ten_maxes[i] = np.absolute(ffts[i][ind])

    # put all the values into a flat array
    features = np.hstack((means, std_dev, skewness, kurtosis, covariances, eigenvalues_of_cov, ten_maxes.flatten()))


    return features


def run_model(x_vals, y):
    # set model parameters
    model = models.Sequential()
    model.add(layers.Conv1D(193, (96), strides=3, activation='relu', input_shape = (1, x_vals.shape[1]), padding='same'))
    model.add(layers.MaxPooling1D(pool_size=(x_vals.shape[1]), padding='same'))
    model.add(layers.Dropout(0.1))
    model.add(layers.Conv1D(212, (5), activation='relu', strides=2, padding='same'))
    model.add(layers.MaxPooling1D(pool_size=(x_vals.shape[1]), padding='same'))
    model.add(layers.Dropout(0.165))
    model.add(layers.Conv1D(109, (5), activation='relu', strides=3, padding='same'))
    model.add(layers.MaxPooling1D(pool_size=(x_vals.shape[1]), padding='same'))
    model.add(layers.Dropout(0.1))
    model.add(layers.Flatten())
    model.add(layers.Dense(256, activation='sigmoid'))
    model.add(layers.Dropout(0.241))
    model.add(layers.Dense(1))

    model.compile(optimizer='adam', loss=tf.keras.losses.MeanSquaredError(), 
            metrics=['MeanSquaredError'])

    model.summary()

    # fit model with 15 epochs
    history = model.fit(x_vals, y, epochs=15)




def get_x(subject_number):
    # get x values for a subject
    # for each interval ie. MANY rows



    file_name = "subject_%.2d.csv" % subject_number
    # generate filename in form subject_01.csv
    df = pd.read_csv(file_name, header=None, index_col=0)

    num_channels  = df.shape[1] - 2
    

    # get array representation of EEG recording from every channel
    channel_data = np.zeros((df.shape[1] - 2, df.shape[0]))
    # shape of channel data = (num_channels, length of recording)
    for i in range(1, len(df.columns)-1):
        # df[i] will select that column, ie. that channel
        channel_data[i - 1] = df[i]

        # this is essentially just transposing the matrix from the csv

    # available timeframe for this dataset is 0.0019531 (at start) to 234 (at end)
    # with roughly 5 rows per each hundredth of a second
    # we want to grab chunks in intervals of 0.5 sec: 
    # that's 50 rows per chunk

    interval = 50
    lower_bound = 0
    upper_bound = interval
    num_intervals = int(df.shape[0] // (interval / 2) - 1)
    assert num_intervals > 0, "error calculating num_intervals. num_intervals <= 0, won't be able to create array"
    
    num_features = 376
    x = np.empty((1 ,num_features))
    slice = channel_data[:, lower_bound : upper_bound]
    x[0] = get_stats(df.shape[1], slice)
    

    for i in range(1, num_intervals):
        lower_bound += (interval // 2)
        upper_bound += (interval // 2)
        # at each iteration, slice channel_data
        slice = channel_data[:, lower_bound: upper_bound]

        features = get_stats(df.shape[1], slice)


        features = features.reshape(1, -1)

        x = np.vstack((x, features))

    assert x.shape[1] == num_features, x.shape
    print("FINISHED ONE FILE! :)")
    return x



def get_y(subject_number):
    fatigue_column = 3
    # prepare the y-value!

    # fatigue level stored in column 3 as a value 0-10

    df_w_labels = pd.read_csv("demographic.csv", header=None, index_col=0)
    y_val = df_w_labels.iloc[subject_number][fatigue_column]

    return y_val



if __name__ == "__main__":
    sub_numbers = [i for i in range(1, 3)]#21)]
    rows = get_x(1)
    labels = np.array([get_y(1)])
    for i in range(1, len(sub_numbers)):
        subject_number = sub_numbers[i]
        rows = np.vstack((rows, get_x(subject_number)))
        labels = np.hstack((labels, get_y(subject_number)))


    # normalize data
    rows = preprocessing.normalize(rows)

    run_model(rows, labels)



    














import pandas as pd
import numpy as np
import math
import statistics
# paper on methodology: https://link.springer.com/chapter/10.1007/978-3-030-29933-0_37


subject_number = 1
file_name = "subject_%.2d.csv" % subject_number
# generate filename in form subject_01.csv
df = pd.read_csv(file_name, header=None, index_col=0)

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
num_label_rows = 2 # the 2 rows at the end aren't important -- used for label info

# df.iloc[i] will grab row i
# df[i] grabs column i

# stats being gathered: sample mean, sample standard deviation, 
# sample skewness, sample kurtosis, 
# sample variances & covariances (for all signal pairs)
# eigenvalues of covariance matrix
# magnitude of frequency components of each signal using a 
# Fast Fourier Transform 

# stats not being gathered: max & min of each signal
# upper triangular elements of matrix log of the cov. matrix
# freq. values of the ten most energetic components of the FFT
# none of the two half-windows & quarter windows values

# i am half-assing this

# 1). find mean
means = np.zeros((df.shape[1] - num_label_rows))
num_channels = df.shape[1] - num_label_rows
for channel in range(num_channels):
    means[channel] = statistics.mean(channel_data[channel])

print(means)


# chunks = np.zeros((num_intervals, df.shape[1] - num_label_rows))
# # each row will be a datapoint for the CNN (y-value will be the level of fatigue of that person)

# # populate chunks
# for i in num_intervals:
#     # do stuff


#     lower_bound += int(interval // 2)
#     upper_bound += int(interval // 2)



# prepare the y-value!
fatigue_column = 3
# fatigue level stored in column 3 as a value 0-10

# df_w_labels = pd.read_csv("demographics.csv", header=None, index=0)
# y_val = df_w_labels.iloc[subject_number][fatigue_column]





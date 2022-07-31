import pandas as pd
from sklearn import svm
import numpy as np
from sklearn.preprocessing import MinMaxScaler
# predict whether a person is feeling positive, negative, or neutral from brain data
# using data from https://www.kaggle.com/datasets/birdy654/eeg-brainwave-dataset-feeling-emotions

# J. J. Bird, L. J. Manso, E. P. Ribiero, A. Ekart, and D. R. Faria, “A study on mental state classification using 
# eeg-based brain-machine interface,”in 9th International Conference on Intelligent Systems, IEEE, 2018.

# J. J. Bird, A. Ekart, C. D. Buckingham, and D. R. Faria, “Mental emotional sentiment classification with 
# an eeg-based brain-machine interface,” in The International Conference on Digital Image and Signal 
# Processing (DISP’19), Springer, 2019.



df = pd.read_csv("emotions.csv")

def encode_labels(df):
    # originally data encoded with "NEGATIVE", "POSITIVE", "NEUTRAL" etc
    # encode with:
    #   neg = 0
    #   pos = 2
    #   neutral = 1
    for i in df.index:
        emotion = df["label"].iloc[i]
        if emotion == "NEGATIVE":
            df.at[i, "label"] = 0
        elif emotion == "POSITIVE":
            df.at[i, "label"] = 2
        elif emotion == "NEUTRAL":
            df.at[i, "label"] = 1

def get_x(df):
    # store data from dataframe into a numpy array
    x = np.zeros((df.shape[0], df.shape[1] - 1))
    for i in df.index:
        new_series = df.iloc[i][:-1]
        x[i] = new_series.tolist()

    return x

def get_y(df):
    # store data from dataframe into numpy array
    y = np.zeros((df.shape[0], ))
    for i in df.index:
        label = df.iloc[i][-1]
        y[i] = label
    
    return y

def run_svc(x_train, y_train, x_test, y_test):
    # fit svc classifier on train data
    # test using testing data
    clf = svm.SVC(kernel='linear', cache_size=7000)
    clf = clf.fit(x_train, y_train)
    print("SCORE: ", clf.score(x_test, y_test))

    y_pred = clf.predict(x_test)
    for val in y_pred:
        print(val)


if __name__ == "__main__":

    # test-train split: 1800 to 332
    train_amt = 1800


    encode_labels(df)
    x = get_x(df)
    # scale x-values between -1 and 1
    scaling = MinMaxScaler(feature_range = (-1, 1)).fit(x)
    x = scaling.transform(x)
    
    y = get_y(df)

    # randomly permutate x and y values, together
    p = np.random.permutation(len(x))
    x, y = x[p], y[p]

    # test - train split
    x_train = x[:train_amt, :]
    y_train = y[:train_amt]
    x_test = x[train_amt:, :]
    y_test = y[train_amt:]


    run_svc(x_train, y_train, x_test, y_test)

    



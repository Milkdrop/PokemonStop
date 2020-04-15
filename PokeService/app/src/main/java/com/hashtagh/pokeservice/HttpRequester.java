package com.hashtagh.pokeservice;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedOutputStream;
import java.io.OutputStreamWriter;
import java.net.*;
import java.io.OutputStream;

public class HttpRequester extends AsyncTask<String,Integer,String> {
    @Override
    protected String doInBackground(String... params) {

        try {
            URL url = new URL (params[0]);
            String token = params[1];
            String data = params[2];

            HttpURLConnection httpURLConnection = (HttpURLConnection) url.openConnection();
            // setting the  Request Method Type
            httpURLConnection.setRequestMethod("POST");
            // adding the headers for request
            httpURLConnection.setRequestProperty("Content-Type", "application/json");
            httpURLConnection.setRequestProperty("Authorization", "Bearer " + token);

            try{
                //to tell the connection object that we will be wrting some data on the server and then will fetch the output result
                httpURLConnection.setDoOutput(true);
                // this is used for just in case we don't know about the data size associated with our request
                httpURLConnection.setChunkedStreamingMode(0);

                // to write tha data in our request
                OutputStream outputStream = new BufferedOutputStream(httpURLConnection.getOutputStream());
                OutputStreamWriter outputStreamWriter = new OutputStreamWriter(outputStream);
                outputStreamWriter.write(data);
                outputStreamWriter.flush();
                outputStreamWriter.close();

                // to log the response code of your request
                Log.i ("Unity", "Server Response: " +httpURLConnection.getResponseCode());

            } catch (Exception e){
                e.printStackTrace();
            } finally {
                httpURLConnection.disconnect();
            }

        }catch (Exception e){
            e.printStackTrace();
        }

        return null;
    }
}

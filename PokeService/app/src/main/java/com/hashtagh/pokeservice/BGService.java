package com.hashtagh.pokeservice;
import android.app.Service;
import android.content.*;
import android.location.Location;
import android.location.LocationManager;
import android.os.*;
import android.util.Log;

public class BGService extends Service {
    public Handler handler = null;
    public static Runnable runnable = null;

    private String token;
    private LocationManager locationManager;

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    public Location getLastBestLocation() {
        Location locationGPS = locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER);
        Location locationNet = locationManager.getLastKnownLocation(LocationManager.NETWORK_PROVIDER);

        long GPSLocationTime = 0;
        if (null != locationGPS) { GPSLocationTime = locationGPS.getTime(); }

        long NetLocationTime = 0;

        if (null != locationNet) {
            NetLocationTime = locationNet.getTime();
        }

        if ( 0 < GPSLocationTime - NetLocationTime ) {
            return locationGPS;
        }
        else {
            return locationNet;
        }
    }

    @Override
    public void onCreate() {
        Log.i ("Unity", "Creating Service...");
        this.locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);

        handler = new Handler();
        runnable = new Runnable() {
            public void run() {
                Log.i ("Unity", "Service Ping");
                Location location = getLastBestLocation ();
                Log.i ("Unity","Latitude: " + location.getLatitude());
                Log.i ("Unity","Longitude: " + location.getLongitude());

                String data = "{\"lat\":\"" + location.getLatitude() + "\", \"long\":\"" + location.getLongitude() + "\"}";

                new HttpRequester().execute ("https://webhook.site/5de62bce-d222-456a-9232-5ba6824e1905/ping", data);
                handler.postDelayed (runnable, 5000);
            }
        };

        handler.postDelayed (runnable, 5000);
        Log.i ("Unity", "Service Created!");
}

    @Override
    public int onStartCommand (Intent intent, int flags, int startId) {
        this.token = intent.getStringExtra ("token");
        return Service.START_REDELIVER_INTENT;
    }

    @Override
    public void onDestroy() {
        Log.i("Unity", "Service Stopped?");
    }
}
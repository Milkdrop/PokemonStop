package com.hashtagh.pokeservice;
import android.app.Notification;
import android.app.PendingIntent;
import android.app.Service;
import android.content.*;
import android.location.Location;
import android.location.LocationManager;
import android.os.*;
import android.support.v4.app.NotificationCompat;
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

                new HttpRequester().execute ("https://api.peymen.com/location", token, data);
                handler.postDelayed (runnable, 10000);
            }
        };

        handler.postDelayed (runnable, 10000);

        /*Intent notificationIntent = new Intent (PingerClass.context, PingerClass.class);
        PendingIntent pendingIntent = PendingIntent.getService (PingerClass.context, 0, notificationIntent, 0);

        Log.i ("Unity","Creating notification");
        Notification notification = new NotificationCompat.Builder (PingerClass.context, PingerClass.notifChannelID)
                .setContentTitle ("Alt text exemplu")
                .setContentText ("Exemplu de continut")
                .setSmallIcon (R.drawable.ic_android)
                .setContentIntent (pendingIntent)
                .build ();

        Log.i ("Unity", "Starting foreground notification...");
        startForeground (1, notification);*/
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
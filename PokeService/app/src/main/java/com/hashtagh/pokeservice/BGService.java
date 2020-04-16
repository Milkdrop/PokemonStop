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
    public Runnable runnable = null;

    public static String token = "";
    public boolean serviceRunning = false;
    public PendingIntent pendingIntent;
    public LocationManager locationManager;
    private SharedPreferences prefMan;

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }

    @Override
    public void onCreate() {
        Log.i ("Unity", "Creating Service...");

        prefMan = this.getSharedPreferences("Pref", 0);
        locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
        handler = new Handler();

        token = prefMan.getString ("serviceToken", "");
        Log.i ("Unity", "Saved token value: " + token);

        if (!token.equals ("")) {
            startService ();
        } else {
            Log.i ("Unity", "Waiting for onStartCommand to set our token");
        }
    }

    @Override
    public int onStartCommand (Intent intent, int flags, int startId) {
        token = intent.getStringExtra ("token");
        pendingIntent = PendingIntent.getService (this, 0, intent, 0);

        SharedPreferences.Editor editor = prefMan.edit();
        editor.putString ("serviceToken", token);
        editor.commit();

        startService ();
        return Service.START_STICKY;
    }

    public void startService () {
        if (serviceRunning) {
            Log.i ("Unity", "Tried to start service when it was already started?");
            return;
        }
        serviceRunning = true;

        handler.removeCallbacks (runnable);
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

        if (Build.VERSION.SDK_INT >= 26) {
            Log.i("Unity", "Creating notification");
            Notification notification = new Notification.Builder(this, "stayHomeChannel")
                    .setContentTitle("#StayHome")
                    .setContentText("Your location is currently being tracked")
                    .setSmallIcon(R.drawable.ic_android)
                    .setContentIntent(pendingIntent)
                    .build();

            startForeground(1, notification);
        }

        Log.i ("Unity", "Service Created!");
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
    public void onDestroy() {
        SharedPreferences.Editor editor = prefMan.edit();
        editor.remove("serviceToken");
        editor.commit();

        Log.i("Unity", "Service Stopped");
        handler.removeCallbacks (runnable);
        serviceRunning = false;
    }
}
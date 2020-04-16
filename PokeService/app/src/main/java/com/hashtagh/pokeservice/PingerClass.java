package com.hashtagh.pokeservice;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Intent;
import android.content.Context;
import android.os.Build;
import android.util.Log;

public class PingerClass {
    public static Context context;
    public static String notifChannelID = "pokeServiceChannel";

    public void initialize (Context _context, String token) {
        this.context = _context;
        Log.i ("Unity", "AAR Started");

        createNotificationChannel ();
        Log.i ("Unity", "Notification Channel created");

        Intent serviceIntent = new Intent (context, BGService.class);
        serviceIntent.putExtra ("token", token);
        context.startService(serviceIntent);

        /*if (Build.VERSION.SDK_INT >= 26) {
            context.startForegroundService(serviceIntent);
        } else {
            context.startService(serviceIntent);
        }*/

        Log.i ("Unity", "Daemon started");
    }

    private void createNotificationChannel () {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel serviceChannel = new NotificationChannel (
                    notifChannelID,
                    "PokeService Channel",
                    NotificationManager.IMPORTANCE_DEFAULT
            );

            NotificationManager manager = context.getSystemService (NotificationManager.class);
            manager.createNotificationChannel (serviceChannel);
        }
    }
}
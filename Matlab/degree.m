clc; clear all; close all;
pn = 4;
a = 0:8:180;
x1 = 343/0.025*sind(a);
figure;
hold on;
plot(x1,a)

a = 0:0.1:180;
x1 = 343/0.10*sind(a);

plot(x1,a)

x2 = 343/0.5 * sind(a);

plot(x2,a)

x3 = 343/1 * sind(a);

plot(x3,a)
hold off;

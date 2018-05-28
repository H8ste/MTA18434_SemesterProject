%RoomImpulseResponse_script
clc; clear all; close all;

mex -setup C++
mex rir_generator.cpp

%Creating orignal signal
fs = 8000;              % Sampling frequency                    
T = 1/fs;             % Sampling period       
L = 4096;               % Length of signal
t = (0:L-1)*T;        % Time vector
frequency = 800;
    
    u=1;
    Original = 2000*sin(2*pi*frequency*u*t);
    %Make isotropic noise
    P = [1.8 1.8];
    %P = [1.132941 1.132941] ;           %P            : sensor positions
    len = L;                                %len          : desired data length
    params.fs = fs;                         %params.fs    : sample frequency
    params.c = 343;                           %params.c     : sound velocity in m/s
    params.N_phi = 256;                   %params.N_phi : number of cylindrical angles
    noise = sinf_1D(P,len,params);
    elementIndex = 1;
    degreeSound= 0:0.5:360;
    
    
    
    for degreeSound=0:0.5:360
    ListAngle = 50;
    DistMicro = 0.075;
    SpeedOfSound = 343;
    Width = 6/2;
    Length = 4/2;
    Heigth = 4/2;
    x = Width + cosd(degreeSound)*1.8;
    y = Length + sind(degreeSound)*1.8;

    %Creating both microphones rir
    c = 340;                    % Sound velocity (m/s)
    %fs = 44100;                 % Sample frequency (samples/s)
    mic_k = [Width-(DistMicro/2) Length Heigth ; Width+(DistMicro/2) Length Heigth];    % Receiver positions [x_1 y_1 z_1 ; x_2 y_2 z_2] (m)
    s = [x y Heigth];              % Source position [x y z] (m)
    L = [Width*2 Length*2 Heigth*2];              % Room dimensions [x y z] (m)
    beta = 0.4;                 % Reverberation time (s)
    n = 4096;                   % Number of samples
    mtype = 'omnidirectional';  % Type of microphone
    order = -1;                 % -1 equals maximum reflection order!
    dim = 3;                    % Room dimension
    orientation = 0;            % Microphone orientation (rad)
    hp_filter = 0;              % Enable high-pass filter

    rir_1 = rir_generator(c, fs, mic_k(1,:), s, L, beta, n, mtype, order, dim, orientation, hp_filter);
    rir_2 = rir_generator(c, fs, mic_k(2,:), s, L, beta, n, mtype, order, dim, orientation, hp_filter);

    sound_rir_mic_1 = filter(rir_1, 1, Original);
    sound_rir_mic_2 = filter(rir_2, 1, Original);

    beamformedSound = beamform(sound_rir_mic_1, sound_rir_mic_2,  DistMicro * cosd(ListAngle) / SpeedOfSound, fs);
    beamformedNoise = beamform(noise(1,:), noise(2,:), DistMicro * cosd(ListAngle) / SpeedOfSound, fs);
    signal_to_noise_ratio(1,elementIndex) = snr(beamformedSound,beamformedNoise);
    variance(1,elementIndex) = var(beamformedSound,1);

    elementIndex = elementIndex + 1;
    end
    elementIndex = 1;
        testing = zeros(2,length(signal_to_noise_ratio),'double');
        testing(1,:) = signal_to_noise_ratio(1,:);

    disp('ran');
    
    
    
    u = 2;
    Original = 2000*sin(2*pi*frequency*u*t);
    
    for degreeSound=0:0.5:360
    ListAngle = 50;
    DistMicro = 0.075;
    SpeedOfSound = 343;
    Width = 6/2;
    Length = 4/2;
    Heigth = 4/2;
    x = Width + cosd(degreeSound)*1.8;
    y = Length + sind(degreeSound)*1.8;

    %Creating both microphones rir
    c = 340;                    % Sound velocity (m/s)
    %fs = 44100;                 % Sample frequency (samples/s)
    mic_k = [Width-(DistMicro/2) Length Heigth ; Width+(DistMicro/2) Length Heigth];    % Receiver positions [x_1 y_1 z_1 ; x_2 y_2 z_2] (m)
    s = [x y Heigth];              % Source position [x y z] (m)
    L = [Width*2 Length*2 Heigth*2];              % Room dimensions [x y z] (m)
    beta = 0.4;                 % Reverberation time (s)
    n = 4096;                   % Number of samples
    mtype = 'omnidirectional';  % Type of microphone
    order = -1;                 % -1 equals maximum reflection order!
    dim = 3;                    % Room dimension
    orientation = 0;            % Microphone orientation (rad)
    hp_filter = 0;              % Enable high-pass filter

    rir_1 = rir_generator(c, fs, mic_k(1,:), s, L, beta, n, mtype, order, dim, orientation, hp_filter);
    rir_2 = rir_generator(c, fs, mic_k(2,:), s, L, beta, n, mtype, order, dim, orientation, hp_filter);

    sound_rir_mic_1 = filter(rir_1, 1, Original);
    sound_rir_mic_2 = filter(rir_2, 1, Original);

    beamformedSound = beamform(sound_rir_mic_1, sound_rir_mic_2,  DistMicro * cosd(ListAngle) / SpeedOfSound, fs);
    beamformedNoise = beamform(noise(1,:), noise(2,:), DistMicro * cosd(ListAngle) / SpeedOfSound, fs);
    signal_to_noise_ratio(1,elementIndex) = snr(beamformedSound,beamformedNoise);
    variance(1,elementIndex) = var(beamformedSound,1);

    elementIndex = elementIndex + 1;
    end

        testing(2,:) = signal_to_noise_ratio(1,:);

    disp('ran');
%subplot(4,1,4);
%plot(0:0.2:360, signal_to_noiseS_ratio);
figure;
degrees = 0:0.5:360;
hold on;
plot(degrees, testing(1,:))
plot(degrees,testing(2,:))
hold off;
xlim([0 360]);
%plot(0:0.2:360, var(beamformedSound));
keyboard;
%ylim([0 60])



% testnoise = transpose(noise);
% ListAngle = 88;
% DistMicro = 0.075;
% SpeedOfSound = 343;
% beamformedSound = beamform(sound_rir_mic_2, sound_rir_mic_1, DistMicro * sin(ListAngle * pi / 180) / SpeedOfSound, fs);
% testing = testnoise(:,1);
% beamformedNoise = beamform(transpose(testnoise(:,1)), transpose(testnoise(:,2)), DistMicro * sin(ListAngle * pi / 180) / SpeedOfSound, fs);
% 
% signal_to_noise_ratio = snr(beamformedSound,beamformedNoise);
% plotnumber = 6;
% currentPlot = 1;
% limitsy = [-100 100];
% limitsx = [0 0.1];
% 
% subplot(plotnumber,1,currentPlot);
% plot(t,beamformedSound);
% currentPlot = currentPlot + 1;
% ylim(limitsy)
% xlim(limitsx)
% title('Beamformed Sound towards 28 degrees, without noise')
% 
% subplot(plotnumber,1,currentPlot);
% plot(t,sound_rir_mic_1);
% currentPlot = currentPlot + 1;
% ylim(limitsy)
% xlim(limitsx)
% title('Microphone 1 input of sound without noise')
% 
% subplot(plotnumber,1,currentPlot);
% plot(t,sound_rir_mic_2);
% currentPlot = currentPlot + 1;
% ylim(limitsy)
% xlim(limitsx)
% title('Microphone 2 input of sound without noise')
% 
% combinedOriginal = (sound_rir_mic_2 + sound_rir_mic_1)/2;
% 
% subplot(plotnumber,1,currentPlot);
% plot(t,combinedOriginal);
% currentPlot = currentPlot + 1;
% ylim(limitsy)
% xlim(limitsx)
% title('Microphone 1 and 2 added together');
% 
% difference = beamformedSound - combinedOriginal;
% 
% subplot(plotnumber, 1, currentPlot);
% plot(t, difference);
% currentPlot = currentPlot + 1;
% ylim(limitsy)
% xlim(limitsx)
% title('Beamformedsound - Microphone 1 and 2 added together')
% 
% %degrees = 28;
% for degrees = 0:1:360
%     beamformedSound = beamform(sound_rir_mic_2, sound_rir_mic_1,  DistMicro * sind(degrees) / SpeedOfSound, fs);
%     beamformedNoise = beamform(transpose(testnoise(:,1)), transpose(testnoise(:,2)), DistMicro * sind(degrees) / SpeedOfSound, fs);
% 
%     signal_to_noise_ratio(1,degrees+1) = snr(beamformedSound,beamformedNoise);
% end
% 
% figure;
% subplot(3,1,1);
% plot(sound_rir_mic_1)
% hold on;
% plot(sound_rir_mic_2);
% hold off;
% 
% degrees = 28;
% beamformedSound = beamform(sound_rir_mic_2, sound_rir_mic_1, DistMicro * sind(degrees) / SpeedOfSound, fs);
% subplot(3,1,2);
% plot(beamformedSound)
% hold on;
% plot(sound_rir_mic_1);
% hold off;
% 
% degrees = 0:1:360;
% subplot(3,1,3);
% plot(degrees,signal_to_noise_ratio);
% xlim([0 360]);
% %ylim([30 33]);



function y = beamform(x1,x2,timedelay, sampleRate)
%%result 1
fftresults = fft(x1);
for idx = 1:numel(fftresults)
    fftshifted(1,idx) = shiftTime(fftresults(1,idx),timedelay,length(fftresults), idx, sampleRate);
end

shiftedMic = ifft(fftshifted, 'symmetric');
%y = ifft(fftshifted, 'symmetric');

y = (x2+shiftedMic)/2;

end

function y = shiftSignal(x1,timedelay, sampleRate)
%%result 1
fftresults = fft(x1);
for idx = 1:numel(fftresults)
    fftshifted(1,idx) = shiftTime(fftresults(1,idx),timedelay,length(fftresults), idx, sampleRate);
end

%shiftedMic = ifft(fftshifted, 'symmetric');
y = ifft(fftshifted, 'symmetric');

%y = (x2+shiftedMic)/2;

end

function g = shiftTime(x1, timedelay, arrayLength, elementIndex, sampleRate)
angle = exp(-1i * ((2 * pi) / arrayLength ) * sampleRate * elementIndex * -1*timedelay);
g = x1 * angle;
end




function z = sinf_1D(d,len,params)

% Generating sensor signals for a 1D sensor array in a spherically 
% isotropic noise field [1]
%
%    z = sinf_1D(d,len,params)
%
% Input parameters:
%    d            : sensor distances
%    len          : desired data length
%    params.fs    : sample frequency
%    params.c     : sound velocity in m/s
%    params.N_phi : number of cylindrical angles
%
% Output parameters:
%    z            : output signal
%
% References:
%    [1] E.A.P. Habets and S. Gannot, 'Generating sensor signals
%        in isotropic noise fields', The Journal of the Acoustical 
%        Society of America, Vol. 122, Issue 6, pp. 3464-3470, Dec. 2007.
%
% Authors:  E.A.P. Habets and S. Gannot
%
% History:  2007-11-02 - Initial version
%           2010-09-16 - Minor corrections
%           2017-20-06 - Use native waitbar
%
% Copyright (C) 2007-2017 E.A.P. Habets
%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%   This program is free software; you can redistribute it and/or modify
%   it under the terms of the GNU General Public License as published by
%   the Free Software Foundation; either version 2 of the License, or
%   (at your option) any later version.
%
%   This program is distributed in the hope that it will be useful,
%   but WITHOUT ANY WARRANTY; without even the implied warranty of
%   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
%   GNU General Public License for more details.
%
%   You can obtain a copy of the GNU General Public License from
%   http://www.gnu.org/copyleft/gpl.html or by writing to
%   Free Software Foundation, Inc.,675 Mass Ave, Cambridge, MA 02139, USA.
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

M = length(d);                        % Number of sensors
NFFT = 2.^ceil(log2(len));             % Number of frequency bins
X = zeros(M,NFFT/2+1);

if ~isfield(params,'fs')
    fs = 8000;                        % Default
else
    fs = params.fs;
end
if ~isfield(params,'c')               
    c = 340;                          % Default
else
    c = params.c;
end
if ~isfield(params,'N_phi')
    N_phi = 64;                       % Default
else
    N_phi = params.N_phi;
end

w = 2*pi*fs*(0:NFFT/2)/NFFT;
phi = acos(2*(0:1/N_phi:1)-1);

% Calculate relative sensor distances
d_rel = d - d(1);

% Initialize waitbar
h = waitbar(0,'Generating sensor signals...');

% Calculate sensor signals in the frequency domain
for phi_idx = 1:N_phi
    waitbar(phi_idx/N_phi);
    X_prime = randn(1,NFFT/2+1) +1i*randn(1,NFFT/2+1);    
    X(1,:) = X(1,:) + X_prime;   
    for m = 2:M
        Delta = d_rel(m)*cos(phi(phi_idx));
        X(m,:) = X(m,:) + X_prime.*exp(-1i*Delta*w/c);
    end    
end
X = X/sqrt(N_phi);

% Transform to time domain
X = [sqrt(NFFT)*real(X(:,1)), sqrt(NFFT/2)*X(:,2:NFFT/2),...
    sqrt(NFFT)*real(X(:,NFFT/2+1)), sqrt(NFFT/2)*conj(X(:,NFFT/2:-1:2))];
z = real(ifft(X,NFFT,2));

% Truncate output signals
z = z(:,1:len);

% Close waitbar
close(h);
end

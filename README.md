# YouTube Music Downloader

A web application that allows users to search YouTube videos, stream them with custom controls, and download them as MP4 or MP3 files.

## Overview

This project consists of two main components:

**MusicPlayerWebApp** - Frontend web application built with ASP.NET Core MVC that provides:
- YouTube video search and streaming
- Interactive video player with time-range selection
- User authentication via AWS Cognito
- Clean web interface for browsing and controlling playback

**MediaService** - Backend API service that handles:
- Video downloading using yt-dlp
- Audio extraction to MP3 format
- File storage in AWS S3
- Pre-signed URL generation for secure downloads

## Key Features

- Search YouTube videos by URL or keywords
- Stream videos with custom playback controls
- Download videos as MP4 or extract audio as MP3
- Time-range selection for video segments
- Secure user authentication
- Cloud storage for downloaded files

## Tech Stack

- **Backend**: ASP.NET Core 8.0, C#
- **Frontend**: HTML/CSS/JavaScript, Bootstrap, jQuery UI
- **External Services**: YouTube Data API v3, AWS S3, AWS Cognito
- **Tools**: yt-dlp for video processing

## Architecture

The application uses a two-tier architecture where the web app handles user interaction and YouTube integration, while the media service manages the heavy lifting of video downloading and storage. Files are processed server-side and stored in AWS S3, with users receiving secure download links.

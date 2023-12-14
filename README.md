# Project Title

This API is based on a take home interview task found
on [Reddit](https://www.reddit.com/r/dotnet/comments/1841x0f/does_this_takehome_project_look_okay/). The application
allows users to search available movies, view showtimes, reserve seats and complete bookings.

## API Documentation

[Documentation](https://linktodocumentation)

## Run Locally

Clone the project and Go to the project directory

```bash
  cd CinemaManagement
```
Run Docker compose

```bash
  docker compose up
```

View the swagger document

```bash
  https://linktodocumentation
```

## Secrets

To run this project, you will need to use the dotnet secrets tool.
The secrets required are:

```json

{
  "": ""
}
```

## Running Tests

To run tests, run the following command in the parent directory.

```bash
  dotnet test
```
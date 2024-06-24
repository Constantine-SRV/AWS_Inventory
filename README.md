This program queries all AWS regions and generates XLS files containing information about the discovered objects. AWS credentials should be saved in a JSON file, and the path to this file should be passed to the program as a parameter.

The JSON file format is as follows:

{
  "AccessKeyId": "AWS_ACCESS_KEY",
  "SecretAccessKey": "AWS_SECRET_KEY"
}

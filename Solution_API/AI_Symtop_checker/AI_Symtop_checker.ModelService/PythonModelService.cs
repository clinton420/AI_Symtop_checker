// AI_Symtop_checker.ModelService/PythonModelService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Python.Runtime;
using System.Text.Json;
using System.Collections;
using Python.Runtime;
using System.Text.Json;


namespace AI_Symtop_checker.ModelService
{
    public class PythonModelService : IModelService, IDisposable
    {
        private readonly ILogger<PythonModelService> _logger;
        private static bool _isInitialized = false;
        private static readonly object _lockObj = new object();
        private bool _disposedValue;

        public PythonModelService(ILogger<PythonModelService> logger)
        {
            _logger = logger;
            Initialize();
        }

        private void Initialize()
        {
            if (!_isInitialized)
            {
                lock (_lockObj)
                {
                    if (!_isInitialized)
                    {
                        try
                        {
                            // Try to find Python path using python itself
                            string pythonPath = FindPythonDllUsingPython();

                            if (string.IsNullOrEmpty(pythonPath))
                            {
                                // Fall back to the existing method if the Python-based approach fails
                                pythonPath = FindPythonDllUsingFileSystem();
                            }

                            if (string.IsNullOrEmpty(pythonPath))
                            {
                                throw new Exception("Python DLL not found. Please ensure Python is installed correctly.");
                            }

                            _logger.LogInformation("Using Python DLL: {PythonPath}", pythonPath);

                            // Set environment variable before initialization
                            Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonPath);

                            // Initialize Python
                            PythonEngine.Initialize();
                            PythonEngine.BeginAllowThreads();

                            _logger.LogInformation("Python runtime initialized successfully");
                            _isInitialized = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to initialize Python runtime");
                            throw;
                        }
                    }
                }
            }
        }

        private string FindPythonDllUsingPython()
        {
            try
            {
                // Execute python to find the correct DLL path
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = "-c \"import sys, os; print(os.path.join(os.path.dirname(sys.executable), 'python' + str(sys.version_info.major) + str(sys.version_info.minor) + '.dll'))\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                if (File.Exists(output))
                {
                    return output;
                }

                // Try to find python3XX.dll in various locations relative to the output path
                string fileName = Path.GetFileName(output);
                string pythonDir = Path.GetDirectoryName(output);

                // Try DLLs directory
                string dllsPath = Path.Combine(pythonDir, "DLLs", fileName);
                if (File.Exists(dllsPath))
                {
                    return dllsPath;
                }

                // Try libs directory
                string libsPath = Path.Combine(pythonDir, "libs", fileName);
                if (File.Exists(libsPath))
                {
                    return libsPath;
                }

                // Check for alternate case (.DLL vs .dll)
                string altCasePath = Path.Combine(pythonDir, fileName.ToUpperInvariant());
                if (File.Exists(altCasePath))
                {
                    return altCasePath;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to find Python DLL using Python command");
                return null;
            }
        }

        private string FindPythonDllUsingFileSystem()
        {
            try
            {
                // Your existing method for finding Python DLL
                string pythonPath = null;

                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "where",
                        Arguments = "python",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output))
                {
                    // Take the first line which should be the main Python executable
                    string pythonExePath = output.Split(Environment.NewLine)[0].Trim();

                    if (!string.IsNullOrEmpty(pythonExePath) && File.Exists(pythonExePath))
                    {
                        // Get the directory containing the Python executable
                        string pythonDir = Path.GetDirectoryName(pythonExePath);

                        _logger.LogInformation("Found Python at: {PythonDir}", pythonDir);

                        // Find Python DLL in the same directory
                        string[] possibleDlls = Directory.GetFiles(pythonDir, "python*.dll");

                        if (possibleDlls.Length > 0)
                        {
                            // Prefer non-debug DLL
                            pythonPath = possibleDlls.FirstOrDefault(p => !Path.GetFileName(p).Contains("_d.")) ?? possibleDlls[0];
                        }
                        else
                        {
                            // Check in the DLLs subfolder (common location)
                            string dllsDir = Path.Combine(pythonDir, "DLLs");
                            if (Directory.Exists(dllsDir))
                            {
                                possibleDlls = Directory.GetFiles(dllsDir, "python*.dll");
                                if (possibleDlls.Length > 0)
                                {
                                    pythonPath = possibleDlls.FirstOrDefault(p => !Path.GetFileName(p).Contains("_d.")) ?? possibleDlls[0];
                                }
                            }

                            // Check in libs folder as a fallback
                            if (pythonPath == null)
                            {
                                string libsDir = Path.Combine(pythonDir, "libs");
                                if (Directory.Exists(libsDir))
                                {
                                    possibleDlls = Directory.GetFiles(libsDir, "python*.dll");
                                    if (possibleDlls.Length > 0)
                                    {
                                        pythonPath = possibleDlls.FirstOrDefault(p => !Path.GetFileName(p).Contains("_d.")) ?? possibleDlls[0];
                                    }
                                }
                            }
                        }
                    }
                }

                // Fallback to environment variable if command failed
                if (string.IsNullOrEmpty(pythonPath))
                {
                    string pythonHome = Environment.GetEnvironmentVariable("PYTHONHOME");
                    if (!string.IsNullOrEmpty(pythonHome))
                    {
                        _logger.LogInformation("Using PYTHONHOME: {PythonHome}", pythonHome);
                        string[] possibleDlls = Directory.GetFiles(pythonHome, "python*.dll");
                        if (possibleDlls.Length > 0)
                        {
                            pythonPath = possibleDlls.FirstOrDefault(p => !Path.GetFileName(p).Contains("_d.")) ?? possibleDlls[0];
                        }
                    }
                }

                // Final fallback to a common location
                if (string.IsNullOrEmpty(pythonPath))
                {
                    _logger.LogWarning("Python not found through automatic detection, trying common locations");
                    string[] commonPaths = new[]
                    {
                        @"C:\Python38\python38.dll",
                        @"C:\Python39\python39.dll",
                        @"C:\Python310\python310.dll",
                        @"C:\Python311\python311.dll",
                        @"C:\Program Files\Python38\python38.dll",
                        @"C:\Program Files\Python39\python39.dll",
                        @"C:\Program Files\Python310\python310.dll",
                        @"C:\Program Files\Python311\python311.dll"
                    };

                    foreach (var path in commonPaths)
                    {
                        if (File.Exists(path))
                        {
                            pythonPath = path;
                            break;
                        }
                    }
                }

                return pythonPath;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to find Python DLL using file system");
                return null;
            }
        }

        public async Task<ModelResponse> AnalyzeSymptomsAsync(ModelRequest request)
        {
            return await Task.Run(() =>
            {
                _logger.LogInformation("Analyzing symptoms: {Symptoms}", string.Join(", ", request.Symptoms));

                try
                {
                    using (Py.GIL())
                    {
                        // Add the script directory to Python path
                        dynamic sys = Py.Import("sys");
                        string scriptDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PythonScripts");
                        sys.path.append(scriptDir);

                        // Import the model module
                        dynamic modelModule = Py.Import("llama_model");

                        // Convert request data to Python
                        using PyList symptoms = new PyList();
                        foreach (var symptom in request.Symptoms)
                        {
                            symptoms.Append(new PyString(symptom));
                        }

                        // Call the analyze_symptoms function
                        dynamic result = modelModule.analyze_symptoms(
                            symptoms,
                            request.PatientAge ?? "",
                            request.PatientGender ?? "",
                            request.AdditionalNotes ?? ""
                        );

                        // Convert Python dictionary to string and then to .NET object

                        dynamic json = Py.Import("json");
                        string jsonResult = json.dumps(result);
                        var resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResult); ;
                        

                        var parsedResult = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResult);

                        // Check for errors
                        if (parsedResult.ContainsKey("error"))
                        {
                            throw new Exception(parsedResult["error"].ToString());
                        }

                        // Map to response model
                        var response = new ModelResponse
                        {
                            Prediction = parsedResult["prediction"].ToString(),
                            Confidence = Convert.ToDouble(parsedResult["confidence"]),
                            UrgencyLevel = parsedResult["urgency_level"].ToString(),
                            AdditionalNotes = parsedResult["additional_notes"].ToString(),
                            Timestamp = DateTime.Parse(parsedResult["timestamp"].ToString())
                        };

                        // Process collections
                        var possibleConditions = JsonSerializer.Deserialize<List<string>>(
                            parsedResult["possible_conditions"].ToString()
                        );
                        response.PossibleConditions = possibleConditions;

                        var recommendedActions = JsonSerializer.Deserialize<List<string>>(
                            parsedResult["recommended_actions"].ToString()
                        );
                        response.RecommendedActions = recommendedActions;

                        return response;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error analyzing symptoms: {Message}", ex.Message);
                    throw;
                }
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // No managed resources to dispose
                }

                // No unmanaged resources to dispose
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
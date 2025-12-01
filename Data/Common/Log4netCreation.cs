using System;
using System.Collections.Generic;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Data
{
    public class Log4netCreation
    {
        ILoggerRepository log4netRepo;
        public ILog GetLogger(string name)
        {
            return LogManager.GetLogger(this.log4netRepo.Name, name);
        }

        public static string LOCAL_LOG_DIR
        {
            get
            {
                return Environment.CurrentDirectory + "/Logs";
            }
        }

        public void Create(string repoName, string logDir, List<string> loggerNamesToAdd, List<bool> releaseModelLogToConsole, XmlElement xmlRoot, bool createExtraError)
        {
            // find templates
            XmlNode consoleAppender = null;
            XmlNode fileAppenderTemplate = null;
            XmlNode loggerTemplate = null;

            XmlNode node = xmlRoot.FirstChild;
            while (node != null)
            {
                if (consoleAppender == null && node.Name == "appender" && node.Attributes["name"].Value == "console")
                {
                    consoleAppender = node;
                }
                if (fileAppenderTemplate == null && node.Name == "appender" && node.Attributes["name"].Value == "file")
                {
                    fileAppenderTemplate = node;
                }
                if (loggerTemplate == null && node.Name == "logger")
                {
                    loggerTemplate = node;
                }
                node = node.NextSibling;
            }

            if (consoleAppender == null || fileAppenderTemplate == null || loggerTemplate == null)
            {
                throw new Exception("init log4net failed 1");
            }

#if RELEASE
            {
                node = loggerTemplate.FirstChild;
                while (node != null)
                {
                    if (node.Name == "level")
                    {
                        node.Attributes["value"].Value = "INFO";
                        break;
                    }
                }
            }

            // if (!releaseModelLogToConsole)
            // {
            //     consoleAppender.ParentNode.RemoveChild(consoleAppender);
            //     node = loggerTemplate.FirstChild;
            //     while (node != null)
            //     {
            //         if (node.Name == "appender-ref" &&  node.Attributes["ref"].Value == "console")
            //         {
            //             node.ParentNode.RemoveChild(node);
            //             break;
            //         }
            //         node = node.NextSibling;
            //     }
            // }
#endif

            // 
            for (int i = 0; i < loggerNamesToAdd.Count; i++)
            {
                string loggerName = loggerNamesToAdd[i];

                // 1
                XmlNode fileAppender = fileAppenderTemplate.Clone();
                fileAppenderTemplate.ParentNode.AppendChild(fileAppender);

                fileAppender.Attributes["name"].Value = "file_" + loggerName;
                bool success = false;
                node = fileAppender.FirstChild;
                while (node != null)
                {
                    if (node.Name == "file")
                    {
                        success = true;
                        node.Attributes["value"].Value = logDir + "/" + loggerName;
                        break;
                    }

                    node = node.NextSibling;
                }
                if (!success)
                {
                    throw new Exception("init log4net failed 2");
                }

                XmlNode importNode = null;
                string errAppenderName = "errfile_" + loggerName;
                if (createExtraError)
                {
                    //增加一个只输出ERROR以上的logger配置
                    XmlNode errFileAppender = fileAppenderTemplate.Clone();
                    fileAppenderTemplate.ParentNode.AppendChild(errFileAppender);
                    errFileAppender.Attributes["name"].Value = errAppenderName;
                    node = errFileAppender.FirstChild;
                    while (node != null)
                    {
                        if (node.Name == "file")
                        {
                            success = true;
                            // 错误日志固定在当前目录下即可
                            node.Attributes["value"].Value = LOCAL_LOG_DIR + "/ERROR/ERR_" + loggerName;
                            break;
                        }

                        node = node.NextSibling;
                    }
                    XmlDocument docFilter = new XmlDocument();
                    docFilter.LoadXml(
                    "<filter type=\"log4net.Filter.LevelRangeFilter\">" +
                    "   <levelMin value = \"ERROR\" />" +
                    "   <levelMax value = \"FATAL\" />" +
                    "</filter>");
                    importNode = errFileAppender.OwnerDocument.ImportNode(docFilter.DocumentElement, true);
                    errFileAppender.AppendChild(importNode);
                }

                // 2
                XmlNode logger = loggerTemplate.Clone();
                loggerTemplate.ParentNode.AppendChild(logger);

                logger.Attributes["name"].Value = loggerName;
                success = false;
                node = logger.FirstChild;
                while (node != null)
                {
                    if (node.Name == "appender-ref" && node.Attributes["ref"].Value == "file")
                    {
                        success = true;
                        node.Attributes["ref"].Value = "file_" + loggerName;

                        if (createExtraError)
                        {
                            XmlDocument docErr = new XmlDocument();
                            docErr.LoadXml($"<appender-ref ref=\"{errAppenderName}\" />");
                            importNode = node.ParentNode.OwnerDocument.ImportNode(docErr.DocumentElement, true);
                            node.ParentNode.AppendChild(importNode);
                        }
                        break;
                    }
                    node = node.NextSibling;
                }

#if RELEASE
                if (!releaseModelLogToConsole[i])
                {
                    node = logger.FirstChild;
                    while (node != null)
                    {
                        if (node.Name == "appender-ref" &&  node.Attributes["ref"].Value == "console")
                        {
                            node.ParentNode.RemoveChild(node);
                            break;
                        }
                        node = node.NextSibling;
                    }
                }
#endif
                if (!success)
                {
                    throw new Exception("init log4net failed 3");
                }
            }

            fileAppenderTemplate.ParentNode.RemoveChild(fileAppenderTemplate);
            loggerTemplate.ParentNode.RemoveChild(loggerTemplate);

            log4netRepo = LogManager.CreateRepository(repoName);
            XmlConfigurator.Configure(log4netRepo, xmlRoot);
        }
    }
}
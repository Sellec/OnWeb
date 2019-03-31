using OnUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Hosting;

namespace OnWeb.CoreBind.Providers
{
    class EmbeddedResourceInfo : Tuple<Assembly, string>
    {
        public EmbeddedResourceInfo(Assembly assembly, string value)
            : base(assembly, value)
        { }
    }

    class EmbeddedResourceFile : VirtualFile
    {
        private EmbeddedResourceInfo mInfo = null;

        public EmbeddedResourceFile(string virtualPath, EmbeddedResourceInfo info)
            : base(virtualPath)
        {
            mInfo = info;
        }

        public override Stream Open()
        {
            return mInfo.Item1.GetManifestResourceStream(mInfo.Item2);
        }

    }

    class EmbeddedResourceProvider : VirtualPathProvider
    {
        private Dictionary<string, string> mPathList = new Dictionary<string, string>();
        private Tuple<string, EmbeddedResourceInfo> mLastAssoc = null;

        public void addTemplateDir(string path, string name = null)
        {
            if (string.IsNullOrEmpty(name)) name = DateTime.Now.ToString();
            mPathList[name] = path;
        }

        private readonly Lazy<EmbeddedResourceInfo[]> _resourceNames =
            new Lazy<EmbeddedResourceInfo[]>(() =>
            {
                var names = new List<EmbeddedResourceInfo>();
                LibraryEnumeratorFactory.Enumerate((assembly) =>
                {
                    foreach (var name in assembly.GetManifestResourceNames())
                        names.Add(new EmbeddedResourceInfo(assembly, name));

                });
                return names.ToArray();
            },
            LazyThreadSafetyMode.ExecutionAndPublication);

        private bool ResourceFileExists(string virtualPath)
        {
            if (virtualPath.Contains("Modules"))
            {
            }

            if (!virtualPath.Contains("/Views/")) return false;
            var virtualPathPrepared = virtualPath.Replace("/", ".");

            mLastAssoc = null;
            foreach (var pair in _resourceNames.Value)
            {
                if (pair.Item2.EndsWith(virtualPathPrepared))
                {
                    mLastAssoc = new Tuple<string, EmbeddedResourceInfo>(virtualPath, pair);
                    return true;
                }
            }

            return false;
        }

        public override bool FileExists(string virtualPath)
        {
            var d = System.Web.HttpContext.Current;
            return base.FileExists(virtualPath) || ResourceFileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (!base.FileExists(virtualPath))
            {
                if (mLastAssoc != null && mLastAssoc.Item1 == virtualPath)
                    return new EmbeddedResourceFile(virtualPath, mLastAssoc.Item2);
            }

            return base.GetFile(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (ResourceFileExists(virtualPath))
            {
                return null;
            }
            else return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

    }
}
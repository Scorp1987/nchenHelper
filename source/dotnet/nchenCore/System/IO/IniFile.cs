////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: IniFile.cs
//FileType: Visual C# Source file
//Author : Nathan Chen
//Created On : 14-Jun-2020
//Last Modified On : 15-Jun-2020
//Description : Class for ini File Related Functions
////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.IO
{
    public class IniFile : IniFile<string>
    {
        public IniFile() : base((string str) => str, (string str) => str) { }

        public IniFile(string filePath, string commentStr = DEF_COMMENT_STRING) : this() { Load(filePath, commentStr); }
    }

    public class IniFile<T>
    {
        private struct RegExList
        {
            public Regex regexComment;
            public Regex regexSection;
            public Regex regexKey;
            public Regex regexKeyNoValue;

            public RegExList(string commentStr = DEF_COMMENT_STRING)
            {
                regexComment = new Regex(@"^[\s]*\" + commentStr + ".*", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
                regexSection = new Regex(@"^[\s]*\[[\s]*([^\[\s].*[^\s\]])[\s]*\][\s]*\" + commentStr + "*.*", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
                regexKey = new Regex(@"^\s*([^=]*[^\s=])\s*=\s*([^\" + commentStr + @"]*[^\s\" + commentStr + @"])\" + commentStr + "*.*", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
                regexKeyNoValue = new Regex(@"^\s*([^=]*[^\s=])\s*=\s*\" + commentStr + "*.*", (RegexOptions.Singleline | RegexOptions.IgnoreCase));
            }
        }

        protected const string DEF_COMMENT_STRING = ";";
        protected const int DEF_KEY_PADDING = 0;
        private readonly Dictionary<string, IniSection> _sections;

        public IniFile(
            Func<string, T> convertFunctionToT,
            Func<T, string> convertFunctionToString,
            string commentStr = DEF_COMMENT_STRING,
            int keyPadding = DEF_KEY_PADDING)
        {
            CommentString = commentStr;
            KeyPadding = keyPadding;
            ConvertFunctionToT = convertFunctionToT;
            ConvertFunctionToString = convertFunctionToString;
            _sections = new Dictionary<string, IniSection>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get the list of iniSection
        /// </summary>
        public ICollection<IniSection> Sections { get => _sections.Values; }
        public ICollection<IniSection.IniKey> Keys
        {
            get
            {
                var keys = from sec in Sections
                           from key in sec.Keys
                           select key;
                return keys.ToList();
            }
        }
        public virtual string CommentString { get; set; }
        public virtual int KeyPadding { get; set; }
        public virtual Func<string, T> ConvertFunctionToT { get; private set; }
        public virtual Func<T, string> ConvertFunctionToString { get; private set; }

        /// <summary>
        /// Remove All Sections
        /// </summary>
        public void Clear()
        {
            RemoveAllSection();
        }


        #region Load from Ini file format using filePath
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        public void Load(string filePath) => Load(filePath, ConvertFunctionToT, CommentString);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        public void Load(string filePath, string commentStr = DEF_COMMENT_STRING) => Load(filePath, ConvertFunctionToT, commentStr);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        /// <param name="convertFunction">Function to Convert from String to T Type</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        public void Load(string filePath, Func<string, T> convertFunction, string commentStr = DEF_COMMENT_STRING)
        {
            using FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Load(stream, convertFunction, commentStr);
        }
        #endregion


        #region Load from Ini file format using stream
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be read from</param>
        public void Load(Stream stream) => Load(stream, ConvertFunctionToT, CommentString);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be read from</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        public void Load(Stream stream, string commentStr = DEF_COMMENT_STRING) => Load(stream, ConvertFunctionToT, commentStr);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be read from</param>
        /// <param name="convertFunction">Function to Convert from String to T Type</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        public virtual void Load(Stream stream, Func<string, T> convertFunction, string commentStr = DEF_COMMENT_STRING)
        {
            RegExList regExList = new RegExList(commentStr);

            IniSection tempSection = null;
            using StreamReader oReader = new StreamReader(stream);
            while (!oReader.EndOfStream)
            {
                string line = oReader.ReadLine();
                tempSection = ProcessLine(line, tempSection, convertFunction, regExList);
            }
            oReader.Close();
        }
        #endregion


        #region LoadAsyn from Ini file format using filePath
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        /// <returns></returns>
        public async Task LoadAsync(string filePath) => await LoadAsync(filePath, ConvertFunctionToT, CommentString);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        /// <returns></returns>
        public async Task LoadAsync(string filePath, string commentStr = DEF_COMMENT_STRING) => await LoadAsync(filePath, ConvertFunctionToT, commentStr);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be readed from</param>
        /// <param name="convertFunction">Function to Convert from String to T Type</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        /// <returns></returns>
        public async Task LoadAsync(string filePath, Func<string, T> convertFunction, string commentStr = DEF_COMMENT_STRING)
        {
            using FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            await LoadAsync(stream, convertFunction, commentStr);
        }
        #endregion


        #region LoadAsyn from Ini file format using stream
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <returns></returns>
        public async Task LoadAsync(Stream stream) => await LoadAsync(stream, ConvertFunctionToT, CommentString);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        /// <returns></returns>
        public async Task LoadAsync(Stream stream, string commentStr = DEF_COMMENT_STRING) => await LoadAsync(stream, ConvertFunctionToT, commentStr);
        /// <summary>
        /// Load from Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="convertFunction">Function to Convert from String to T Type</param>
        /// <param name="commentStr">Comment String to be ignore</param>
        /// <returns></returns>
        public virtual async Task LoadAsync(Stream stream, Func<string, T> convertFunction, string commentStr = DEF_COMMENT_STRING)
        {
            RegExList regExList = new RegExList(commentStr);

            IniSection tempSection = null;
            using StreamReader oReader = new StreamReader(stream);
            while (!oReader.EndOfStream)
            {
                string line = await oReader.ReadLineAsync();
                tempSection = ProcessLine(line, tempSection, convertFunction, regExList);
            }
            oReader.Close();
        }
        #endregion


        private IniSection ProcessLine(string line, IniSection tempSection, Func<string, T> convertFunction, RegExList regExList)
        {
            if (line.Trim() == string.Empty)
            {
                Trace.WriteLine("Skipping blank line");
                return tempSection;
            }

            Match m;
            m = regExList.regexComment.Match(line);
            if (m.Success)
            {
                Trace.WriteLine("Skipping Comment: " + m.Groups[0].Value);
                return tempSection;
            }

            m = regExList.regexSection.Match(line);
            if (m.Success)
            {
                Trace.WriteLine("Adding section [" + m.Groups[1].Value + ']');
                tempSection = AddOrGetSection(m.Groups[1].Value);
                return tempSection;
            }

            if (tempSection == null)
            {
                Trace.WriteLine("Skipping line without section: " + line);
                return tempSection;
            }

            m = regExList.regexKey.Match(line);
            if (m.Success)
            {
                Trace.WriteLine("Adding Key " + m.Groups[1].Value + '=' + m.Groups[2].Value);
                tempSection.AddOrGetKey(m.Groups[1].Value).Value = convertFunction(m.Groups[2].Value);
                return tempSection;
            }

            m = regExList.regexKeyNoValue.Match(line);
            if (m.Success)
            {
                Trace.WriteLine("Adding Key ", m.Groups[1].Value);
                tempSection.AddOrGetKey(m.Groups[1].Value);
                return tempSection;
            }

            Trace.WriteLine("Adding Key ", line);
            tempSection.AddOrGetKey(line);
            return tempSection;
        }


        #region Save to Ini file format using filePath
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        public void Save(string filePath) => Save(filePath, ConvertFunctionToString, KeyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        public void Save(string filePath, int keyPadding = DEF_KEY_PADDING) => Save(filePath, ConvertFunctionToString, keyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        /// <param name="convertFunction">Function to Convert from T Type to String</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        public void Save(string filePath, Func<T, string> convertFunction, int keyPadding = DEF_KEY_PADDING)
        {
            using FileStream stream = new FileStream(filePath, FileMode.Create);
            Save(stream, convertFunction, keyPadding);
        }
        #endregion


        #region Save to Ini file format using stream
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        public void Save(Stream stream) => Save(stream, ConvertFunctionToString, KeyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        public void Save(Stream stream, int keyPadding = DEF_KEY_PADDING) => Save(stream, ConvertFunctionToString, keyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="convertFunction">Function to Convert from T Type to String</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        public virtual void Save(Stream stream, Func<T, string> convertFunction, int keyPadding = DEF_KEY_PADDING)
        {
            StreamWriter oWriter = new StreamWriter(stream);
            foreach (IniSection s in Sections)
            {
                //Trace.WriteLine("Writing Section: " + s.Name);
                oWriter.WriteLine("\r\n[" + s.Name + ']');
                foreach (IniSection.IniKey k in s.Keys)
                {
                    string toWrite = k.Name.PadRight(keyPadding);
                    if (k.Value?.ToString() != string.Empty) toWrite += " = " + convertFunction(k.Value);
                    //Trace.WriteLine("Writing Key: " + toWrite);
                    oWriter.WriteLine(toWrite);
                }
            }
            oWriter.Flush();
        }
        #endregion


        #region SaveAsync to Ini file format using filePath
        /// <summary>
        /// Save to Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        /// <returns></returns>
        public async Task SaveAsync(string filePath) => await SaveAsync(filePath, ConvertFunctionToString, KeyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        /// <returns></returns>
        public async Task SaveAsync(string filePath, int keyPadding = DEF_KEY_PADDING) => await SaveAsync(filePath, ConvertFunctionToString, keyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="filePath">File Path to be saved to</param>
        /// <param name="convertFunction">Function to Convert from T Type to String</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        /// <returns></returns>
        public async Task SaveAsync(string filePath, Func<T, string> convertFunction, int keyPadding = DEF_KEY_PADDING)
        {
            using FileStream stream = new FileStream(filePath, FileMode.Create);
            await SaveAsync(stream, convertFunction, keyPadding);
        }
        #endregion


        #region SaveAsyn to Ini file format using stream
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <returns></returns>
        public async Task SaveAsync(Stream stream) => await SaveAsync(stream, ConvertFunctionToString, KeyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        /// <returns></returns>
        public async Task SaveAsync(Stream stream, int keyPadding = DEF_KEY_PADDING) => await SaveAsync(stream, ConvertFunctionToString, keyPadding);
        /// <summary>
        /// Save To Ini File Format
        /// </summary>
        /// <param name="stream">Stream to be written to</param>
        /// <param name="convertFunction">Function to Convert from T Type to String</param>
        /// <param name="keyPadding">Right Padding to the key</param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Stream stream, Func<T, string> convertFunction, int keyPadding = DEF_KEY_PADDING)
        {
            StreamWriter oWriter = new StreamWriter(stream);
            foreach (IniSection s in Sections)
            {
                Trace.WriteLine("Writing Section: " + s.Name);
                await oWriter.WriteLineAsync("\r\n[" + s.Name + ']');
                foreach (IniSection.IniKey k in s.Keys)
                {
                    string toWrite = k.Name.PadRight(keyPadding);
                    if (k.Value.ToString() != string.Empty) toWrite += " = " + convertFunction(k.Value);
                    Trace.WriteLine("Writing Key: " + toWrite);
                    await oWriter.WriteLineAsync(toWrite);
                }
            }
            await oWriter.FlushAsync();
        }
        #endregion


        /// <summary>
        /// Search and get the section
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="iniSection">Founded iniSection object. If it is not founded, return null.</param>
        /// <returns>True if iniSection was founded otherwise false.</returns>
        public bool TryGetSection(string section, out IniSection iniSection)
        {
            iniSection = null;
            if (section == null) return false;
            return _sections.TryGetValue(section.Trim(), out iniSection);
        }

        public IniSection GetSection(string section) => _sections[section];

        /// <summary>
        /// Search and get the key
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="key">key name to be searched</param>
        /// <param name="iniKey">Founded iniKey object. If it is not founded, return null</param>
        /// <returns>True if iniSection was founded otherwise false.</returns>
        public bool TryGetKey(string section, string key, out IniSection.IniKey iniKey)
        {
            iniKey = null;
            if (!TryGetSection(section, out IniSection s))
                return false;
            return s.TryGetKey(key, out iniKey);
        }

        public IniSection.IniKey GetKey(string section, string key) => _sections[section].GetKey(key);

        /// <summary>
        /// Search and get the value
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="key">key name to be searched</param>
        /// <param name="value">key value to be return</param>
        /// <returns>True if successful otherwise return false</returns>
        public bool TryGetValue(string section, string key, out T value)
        {
            TryGetKey(section, key, out IniSection.IniKey k);
            if (k == null)
                value = default;
            else
                value = k.Value;
            return k != null;
        }

        public T GetValue(string section, string key) => _sections[section].GetValue(key);

        /// <summary>
        /// Search and rename the section
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="newSection">new section name</param>
        /// <returns>true if searching and renaming successfully. otherwise false.</returns>
        public bool TryRenameSection(string section, string newSection)
        {
            if (!TryGetSection(section, out IniSection s))
                return false;
            return s.TrySetName(newSection);
        }

        /// <summary>
        /// Search and rename the key
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="key">key name to be searched</param>
        /// <param name="newKey">new key name</param>
        /// <returns>true if searching and renaming successfully. otherwise false.</returns>
        public bool TryRenameKey(string section, string key, string newKey)
        {
            if (!TryGetKey(section, key, out IniSection.IniKey k))
                return false;
            return k.TrySetName(newKey);
        }

        /// <summary>
        /// Assign value to given section and key
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <param name="key">key name to be searched</param>
        /// <param name="value">key value to be updated</param>
        /// <returns>true if successfully updated, otherwise return false</returns>
        public bool TrySetValue(string section, string key, T value)
        {
            if (!TryGetKey(section, key, out IniSection.IniKey k))
                return false;
            k.Value = value;
            return true;
        }

        /// <summary>
        /// Add additional section if section not exist otherwise get the existing section
        /// </summary>
        /// <param name="section">section name to be searched</param>
        /// <returns>Added or found iniSection object.</returns>
        public IniSection AddOrGetSection(string section)
        {
            if (section == null) return null;
            section = section.Trim();
            if (section.Length < 1) return null;

            if (TryGetSection(section, out IniSection s))
                return s;

            IniSection addedSection = new IniSection(this, section);
            _sections.Add(section, addedSection);
            return addedSection;
        }

        /// <summary>
        /// Remove section
        /// </summary>
        /// <param name="section">iniSection object to be removed</param>
        /// <returns>true if successfully removed. otherewise false.</returns>
        public bool RemoveSection(IniSection section) { return RemoveSection(section.Name); }
        /// <summary>
        /// Remove section
        /// </summary>
        /// <param name="section">section name to be removed</param>
        /// <returns>true if successfully removed. otherewise false.</returns>
        public bool RemoveSection(string section)
        {
            if (section == null) return false;
            return _sections.Remove(section.Trim());
        }

        /// <summary>
        /// Remove key from section
        /// </summary>
        /// <param name="section">section name from which they will be removed</param>
        /// <param name="key">key name to be removed</param>
        /// <returns>true if successfully removed. otherwise false.</returns>
        public bool RemoveKey(string section, string key)
        {
            if (!TryGetSection(section, out IniSection s))
                return false;
            return s.RemoveKey(key);
        }

        /// <summary>
        /// Remove all sections
        /// </summary>
        /// <returns>true if sucessfully removed. otherwise false.</returns>
        public bool RemoveAllSection()
        {
            _sections.Clear();
            return (_sections.Count == 0);
        }

        public class IniSection
        {
            //private readonly IniFile<T> _parent;
            private readonly Dictionary<string, IniKey> _keys = new Dictionary<string, IniKey>();

            protected internal IniSection(IniFile<T> parent, string sectionName)
            {
                IniFile = parent;
                Name = sectionName;
            }

            public IniFile<T> IniFile { get; }

            /// <summary>
            /// Get the section name
            /// </summary>
            public string Name { get; private set; }
            /// <summary>
            /// Get the list of iniKeys
            /// </summary>
            public ICollection<IniKey> Keys { get => _keys.Values; }


            /// <summary>
            /// Search and get the key from current section
            /// </summary>
            /// <param name="key">keyname to be searched</param>
            /// <param name="iniKey">Founded iniKey object. If it is not founded, return null</param>
            /// <returns>True if iniSection was founded otherwise false.</returns>
            public bool TryGetKey(string key, out IniKey iniKey)
            {
                iniKey = null;
                if (key == null) return false;
                return _keys.TryGetValue(key.Trim(), out iniKey);
            }

            public IniKey GetKey(string key) => _keys[key];

            /// <summary>
            /// Search and get the value
            /// </summary>
            /// <param name="key">key name to be searched</param>
            /// <param name="value">key value to be return</param>
            /// <returns>True if successful otherwise return false</returns>
            public bool TryGetValue(string key, out T value)
            {
                TryGetKey(key, out IniKey k);
                if (k == null)
                    value = default;
                else
                    value = k.Value;
                return k != null;
            }

            public T GetValue(string key) => _keys[key].Value;

            /// <summary>
            /// Add additional key if key not exist otherwise get the existing key
            /// </summary>
            /// <param name="key">key name to be searched</param>
            /// <returns>Added or found iniKey object.</returns>
            public IniKey AddOrGetKey(string key)
            {
                if (key == null) return null;
                key = key.Trim();
                if (key.Length < 1) return null;

                if (TryGetKey(key, out IniKey k))
                    return k;

                IniKey addedKey = new IniKey(this, key);
                _keys.Add(key, addedKey);
                return addedKey;
            }

            /// <summary>
            /// Remove key from current section
            /// </summary>
            /// <param name="key">key name to be removed</param>
            /// <returns>true if successfully removed. otherwise false.</returns>
            public bool RemoveKey(string key)
            {
                if (key == null) return false;
                return _keys.Remove(key.Trim());
            }
            /// <summary>
            /// Remove key from current section
            /// </summary>
            /// <param name="key">iniKey object to be removed</param>
            /// <returns>true if successfully removed. otherwise false.</returns>
            public bool RemoveKey(IniKey key)
            {
                return RemoveKey(key.Name);
            }

            /// <summary>
            /// Remove all keys from current section
            /// </summary>
            /// <returns>true if successfully removed. otherwise false.</returns>
            public bool RemoveAllKey()
            {
                _keys.Clear();
                return (_keys.Count == 0);
            }

            /// <summary>
            /// Rename current section name
            /// </summary>
            /// <param name="section">new section name</param>
            /// <returns>true if there is no duplicate found for new section name</returns>
            public bool TrySetName(string section)
            {
                if (section == null) return false;
                section = section.Trim();
                if (section.Length < 1) return false;

                if (!IniFile.TryGetSection(section, out IniSection s))
                {
                    if (!IniFile.RemoveSection(this)) return false;
                    this.Name = section;
                    IniFile._sections.Add(section, this);
                }
                else
                {
                    if (s == this)
                        this.Name = section;
                    else
                        return false;
                }
                return true;
            }

            /// <summary>
            /// Return formatted string
            /// </summary>
            /// <returns>string in [section name] format</returns>
            public override string ToString()
            {
                return '[' + Name + ']';
            }

            public class IniKey
            {
                //private readonly IniSection _parent;

                protected internal IniKey(IniSection parent, string key)
                {
                    Section = parent;
                    Name = key;
                }

                public IniSection Section { get; }

                /// <summary>
                /// Get the name of current key
                /// </summary>
                public string Name { get; private set; }
                /// <summary>
                /// Get and Set the value of current key
                /// </summary>
                public T Value { get; set; }

                /// <summary>
                /// Rename current key name
                /// </summary>
                /// <param name="key">new key name</param>
                /// <returns>true if there is no duplicate found for new key name</returns>
                public bool TrySetName(string key)
                {
                    if (key == null) return false;
                    key = key.Trim();
                    if (key.Length < 1) return false;

                    if (!Section.TryGetKey(key, out IniKey k))
                    {
                        if (!Section.RemoveKey(this)) return false;
                        this.Name = key;
                        Section._keys.Add(key, this);
                    }
                    else
                    {
                        if (k == this)
                            this.Name = key;
                        else
                            return false;
                    }
                    return true;
                }

                /// <summary>
                /// Return formatted string
                /// </summary>
                /// <returns>string in KeyName=Value format</returns>
                public override string ToString()
                {
                    return Name + '=' + Value;
                }
            }
        }
    }
}

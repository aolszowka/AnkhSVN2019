// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Ankh.Scc;
using Ankh.Scc.UI;
using Ankh.VS.WpfServices;
using SharpSvn;

namespace Ankh.UI.Annotate
{
    class AnnotateSource : AnkhPropertyGridItem, IAnnotateSection, ISvnRepositoryItem, ISvnLogItem, INotifyPropertyChanged
    {
        private readonly SvnBlameEventArgs      _args ;
        private readonly SvnOrigin              _origin ;
        private readonly IAnkhServiceProvider   _context ;

        private string _logMessage;
        private bool   _isSelected = false ;

        public bool                     IsSelected { get => _isSelected ; set => SetProperty ( ref _isSelected, value ) ; }
        public IAnkhServiceProvider     Context    { get => _context ; }

        public AnnotateSource ( SvnBlameEventArgs blameArgs, SvnOrigin origin, IAnkhServiceProvider Context )
        {
            _args    = blameArgs ;
            _origin  = origin ;
            _context = Context ;
        }

        [Category("Subversion")]
        public long Revision
        {
            get { return _args.Revision; }
        }

        [Category("Subversion")]
        public string Author
        {
            get { return _args.Author; }
        }

        [Category("Subversion")]
        public DateTime Time
        {
            get { return _args.Time.ToLocalTime(); }
        }

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        [Browsable(false)]
        public string LogMessage
        {
            get
            {
                if (_logMessage == null)
                {
                    if (_args.RevisionProperties != null && _args.RevisionProperties.Contains(SvnPropertyNames.SvnLog))
                    {
                        _logMessage = _args.RevisionProperties[SvnPropertyNames.SvnLog].StringValue ?? "";
                    }
                    else
                        _logMessage = "";
                }
                return _logMessage;
            }
        }

        protected override string ClassName
        {
            get { return string.Format(CultureInfo.InvariantCulture, "r{0}", Revision); }
        }

        protected override string ComponentName
        {
            get { return Origin.Target.FileName; }
        }

        #region ISvnRepositoryItem Members

        Uri ISvnRepositoryItem.Uri
        {
            get { return Origin.Uri; }
        }

        SvnNodeKind ISvnRepositoryItem.NodeKind
        {
            get { return SvnNodeKind.File; }
        }

        SvnRevision ISvnRepositoryItem.Revision
        {
            get { return Revision >= 0 ? Revision : SvnRevision.Working; }
        }

        public void RefreshItem(bool refreshParent)
        {
            // Ignore
        }

        #endregion Members

        #region ISvnLogItem
        IEnumerable<VS.TextMarker> ISvnLogItem.Issues
        {
            get { return new VS.TextMarker[0]; }
        }

        int ISvnLogItem.Index
        {
            get { return -1; }
        }

        KeyedCollection<string, SvnChangeItem> ISvnLogItem.ChangedPaths
        {
            get { return null; }
        }

        Uri ISvnLogItem.RepositoryRoot
        {
            get { return Origin.RepositoryRoot; }
        }
        #endregion Members

        #region BindableBase

        // This class already has a base class, so I have copied the implementation of BindableBase.
        // Later, I hope to remove the Winforms implementation of the annotate function and remove
        // the current base class.

        /// <summary>
        /// Event generated when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the property and raise OnPropertyChanged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
          if (object.Equals(storage, value))
            return false;

          storage = value;
          this.OnPropertyChanged(propertyName);

          return true;
        }

        /// <summary>
        /// Called when property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property (Auto detected by [CallerMemberName]).</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

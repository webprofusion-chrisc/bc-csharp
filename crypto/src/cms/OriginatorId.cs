using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509.Store;

namespace Org.BouncyCastle.Cms
{
    /**
    * a basic index for an originator.
    */
    public class OriginatorID
        : X509CertStoreSelector
    {
        public override int GetHashCode()
        {
            int code = Arrays.GetHashCode(this.SubjectKeyIdentifier);

			BigInteger serialNumber = this.SerialNumber;
			if (serialNumber != null)
            {
                code ^= serialNumber.GetHashCode();
            }

			X509Name issuer = this.Issuer;
            if (issuer != null)
            {
                code ^= issuer.GetHashCode();
            }

			return code;
        }

        public override bool Equals(
            object obj)
        {
			if (obj == this)
				return false;

			OriginatorID id = obj as OriginatorID;

			if (id == null)
				return false;

			return Arrays.AreEqual(SubjectKeyIdentifier, id.SubjectKeyIdentifier)
				&& Objects.Equals(SerialNumber, id.SerialNumber)
				&& IssuersMatch(Issuer, id.Issuer);
        }
    }
}

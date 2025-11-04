import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  IconButton,
  Stack,
  Typography,
  Box,
  Snackbar,
  Alert,
  Divider,
  Tooltip,
} from '@mui/material';
import {
  ContentCopy as CopyIcon,
  Facebook as FacebookIcon,
  Twitter as TwitterIcon,
  LinkedIn as LinkedInIcon,
  Code as CodeIcon,
} from '@mui/icons-material';
import { sharingService } from '../../services/api/sharing/sharingService';
import type { ShareLinkResponse } from '../../types/sharing';

interface ShareDialogProps {
  open: boolean;
  onClose: () => void;
  routeId: string;
  routeName: string;
}

const ShareDialog: React.FC<ShareDialogProps> = ({ open, onClose, routeId, routeName }) => {
  const [shareData, setShareData] = useState<ShareLinkResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [showEmbedCode, setShowEmbedCode] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '' });

  React.useEffect(() => {
    if (open && !shareData) {
      generateShareLink();
    }
  }, [open, routeId]);

  const generateShareLink = async () => {
    setLoading(true);
    try {
      const data = await sharingService.generateShareLink(routeId);
      setShareData(data);
    } catch (error) {
      console.error('Failed to generate share link:', error);
      setSnackbar({ open: true, message: 'Failed to generate share link' });
    } finally {
      setLoading(false);
    }
  };

  const handleCopyLink = async () => {
    if (shareData) {
      const success = await sharingService.copyShareLink(shareData.shareUrl);
      setSnackbar({
        open: true,
        message: success ? 'Link copied to clipboard!' : 'Failed to copy link',
      });
    }
  };

  const handleCopyEmbedCode = async () => {
    if (shareData?.embedCode) {
      try {
        await navigator.clipboard.writeText(shareData.embedCode);
        setSnackbar({ open: true, message: 'Embed code copied to clipboard!' });
      } catch (error) {
        setSnackbar({ open: true, message: 'Failed to copy embed code' });
      }
    }
  };

  const handleSocialShare = (platform: 'facebook' | 'twitter' | 'linkedin') => {
    if (shareData) {
      sharingService.shareOnSocialMedia(platform, shareData.shareUrl, routeName);
    }
  };

  return (
    <>
      <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
        <DialogTitle>Share Route: {routeName}</DialogTitle>
        <DialogContent>
          <Stack spacing={3} sx={{ mt: 1 }}>
            {/* Share Link */}
            <Box>
              <Typography variant="subtitle2" gutterBottom>
                Share Link
              </Typography>
              <Stack direction="row" spacing={1}>
                <TextField
                  fullWidth
                  size="small"
                  value={shareData?.shareUrl || ''}
                  InputProps={{
                    readOnly: true,
                  }}
                  disabled={loading}
                />
                <Tooltip title="Copy link">
                  <IconButton
                    onClick={handleCopyLink}
                    disabled={!shareData || loading}
                    color="primary"
                  >
                    <CopyIcon />
                  </IconButton>
                </Tooltip>
              </Stack>
            </Box>

            {/* Social Media Buttons */}
            <Box>
              <Typography variant="subtitle2" gutterBottom>
                Share on Social Media
              </Typography>
              <Stack direction="row" spacing={1}>
                <Tooltip title="Share on Facebook">
                  <IconButton
                    onClick={() => handleSocialShare('facebook')}
                    disabled={!shareData}
                    sx={{ color: '#1877F2' }}
                  >
                    <FacebookIcon />
                  </IconButton>
                </Tooltip>
                <Tooltip title="Share on Twitter">
                  <IconButton
                    onClick={() => handleSocialShare('twitter')}
                    disabled={!shareData}
                    sx={{ color: '#1DA1F2' }}
                  >
                    <TwitterIcon />
                  </IconButton>
                </Tooltip>
                <Tooltip title="Share on LinkedIn">
                  <IconButton
                    onClick={() => handleSocialShare('linkedin')}
                    disabled={!shareData}
                    sx={{ color: '#0A66C2' }}
                  >
                    <LinkedInIcon />
                  </IconButton>
                </Tooltip>
              </Stack>
            </Box>

            <Divider />

            {/* Embed Code */}
            <Box>
              <Stack direction="row" alignItems="center" spacing={1} sx={{ mb: 1 }}>
                <Typography variant="subtitle2">Embed Code</Typography>
                <Tooltip title="Toggle embed code">
                  <IconButton
                    size="small"
                    onClick={() => setShowEmbedCode(!showEmbedCode)}
                  >
                    <CodeIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              </Stack>
              {showEmbedCode && (
                <Stack spacing={1}>
                  <TextField
                    fullWidth
                    multiline
                    rows={4}
                    size="small"
                    value={shareData?.embedCode || ''}
                    InputProps={{
                      readOnly: true,
                    }}
                    disabled={loading}
                  />
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<CopyIcon />}
                    onClick={handleCopyEmbedCode}
                    disabled={!shareData?.embedCode}
                  >
                    Copy Embed Code
                  </Button>
                </Stack>
              )}
            </Box>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose}>Close</Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={3000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert severity="success" onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </>
  );
};

export default ShareDialog;
